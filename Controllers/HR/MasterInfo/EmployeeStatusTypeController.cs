using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;
using OfficeOpenXml;

namespace Exampler_ERP.Controllers.HR.MasterInfo
{
   public class EmployeeStatusTypeController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
private readonly IHubContext<NotificationHub> _hubContext;


    public EmployeeStatusTypeController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
_hubContext = hubContext;
 
    }
    public async Task<IActionResult> Index(string searchEmployeeStatusTypeName)
    {

      var EmployeeStatusTypesQuery = _appDBContext.Settings_EmployeeStatusTypes
          .Where(b => b.DeleteYNID != 1);

      if (!string.IsNullOrEmpty(searchEmployeeStatusTypeName))
      {
        EmployeeStatusTypesQuery = EmployeeStatusTypesQuery.Where(b => b.EmployeeStatusTypeName.Contains(searchEmployeeStatusTypeName));
      }

      var EmployeeStatusTypes = await EmployeeStatusTypesQuery.ToListAsync();

      if (!string.IsNullOrEmpty(searchEmployeeStatusTypeName) && EmployeeStatusTypes.Count == 0)
      {
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "No Employee Status Type found with the name '" + searchEmployeeStatusTypeName + "'. Please check the name and try again.");
      }

      return View("~/Views/HR/MasterInfo/EmployeeStatusType/EmployeeStatusType.cshtml", EmployeeStatusTypes);
    }
 
    public async Task<IActionResult> EmployeeStatusType()
    {
      var EmployeeStatusTypes = await _appDBContext.Settings_EmployeeStatusTypes.ToListAsync();
      return Ok(EmployeeStatusTypes);
    }// Add the Edit action
    public async Task<IActionResult> Edit(int id)
    {
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      var EmployeeStatusType = await _appDBContext.Settings_EmployeeStatusTypes.FindAsync(id);
      if (EmployeeStatusType == null)
      {
        return NotFound();
      }
      return PartialView("~/Views/HR/MasterInfo/EmployeeStatusType/EditEmployeeStatusType.cshtml", EmployeeStatusType);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Settings_EmployeeStatusType EmployeeStatusType)
    {
      if (ModelState.IsValid)
      {
        if (string.IsNullOrEmpty(EmployeeStatusType.EmployeeStatusTypeName))
        {
          return Json(new { success = false, message = "EmployeeStatusType Name field is required. Please enter a valid text value." });
        }
        _appDBContext.Update(EmployeeStatusType);
        await _appDBContext.SaveChangesAsync();
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Employee Status Type Updated successfully.");
        return Json(new { success = true });
      }
      return Json(new { success = false, message = "Error creating EmployeeStatusType. Please check the inputs." });
    }
    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      return PartialView("~/Views/HR/MasterInfo/EmployeeStatusType/AddEmployeeStatusType.cshtml", new Settings_EmployeeStatusType());
    }

    [HttpPost]
    public async Task<IActionResult> Create(Settings_EmployeeStatusType EmployeeStatusType)
    {
      if (ModelState.IsValid)
      {
        if (string.IsNullOrEmpty(EmployeeStatusType.EmployeeStatusTypeName))
        {
          return Json(new { success = false, message = "EmployeeStatusType Name field is required. Please enter a valid text value." });
        }
        EmployeeStatusType.DeleteYNID = 0;
        _appDBContext.Settings_EmployeeStatusTypes.Add(EmployeeStatusType);
        await _appDBContext.SaveChangesAsync();
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Employee Status Type Created successfully.");
        return Json(new { success = true });
      }
      return Json(new { success = false, message = "Error creating EmployeeStatusType. Please check the inputs." });
    }
    public async Task<IActionResult> Delete(int id)
    {
      var EmployeeStatusType = await _appDBContext.Settings_EmployeeStatusTypes.FindAsync(id);
      if (EmployeeStatusType == null)
      {
        return NotFound();
      }

      EmployeeStatusType.ActiveYNID = 2;
      EmployeeStatusType.DeleteYNID = 1;

      _appDBContext.Settings_EmployeeStatusTypes.Update(EmployeeStatusType);
      await _appDBContext.SaveChangesAsync();
      await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Employee Status Type Deleted successfully.");
      return Json(new { success = true });
    }
    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var EmployeeStatusTypees = await _appDBContext.Settings_EmployeeStatusTypes
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();

      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add("EmployeeStatusTypees");
        worksheet.Cells["A1"].Value = "EmployeeStatusType ID";
        worksheet.Cells["B1"].Value = "EmployeeStatusType Name";
        worksheet.Cells["C1"].Value = "Active";


        for (int i = 0; i < EmployeeStatusTypees.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = EmployeeStatusTypees[i].EmployeeStatusTypeID;
          worksheet.Cells[i + 2, 2].Value = EmployeeStatusTypees[i].EmployeeStatusTypeName;
          worksheet.Cells[i + 2, 3].Value = EmployeeStatusTypees[i].ActiveYNID == 1 ? "Yes" : "No";
        }

        worksheet.Cells["A1:C1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = $"EmployeeStatusTypees-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
    public async Task<IActionResult> Print()
    {
      var EmployeeStatusTypees = await _appDBContext.Settings_EmployeeStatusTypes
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();
      return View("~/Views/HR/MasterInfo/EmployeeStatusType/PrintEmployeeStatusTypes.cshtml", EmployeeStatusTypees);
    }

  }
}

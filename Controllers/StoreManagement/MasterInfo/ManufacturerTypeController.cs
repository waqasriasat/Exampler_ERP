using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;
using OfficeOpenXml;

namespace Exampler_ERP.Controllers.StoreManagement.MasterInfo
{
   public class ManufacturerTypeController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
private readonly IHubContext<NotificationHub> _hubContext;


    public ManufacturerTypeController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
_hubContext = hubContext;
 
    }
    public async Task<IActionResult> Index(string searchManufacturerTypeName)
    {

      var ManufacturerTypesQuery = _appDBContext.Settings_ManufacturerTypes
          .Where(b => b.DeleteYNID != 1);

      if (!string.IsNullOrEmpty(searchManufacturerTypeName))
      {
        ManufacturerTypesQuery = ManufacturerTypesQuery.Where(b => b.ManufacturerTypeName.Contains(searchManufacturerTypeName));
      }

      var ManufacturerTypes = await ManufacturerTypesQuery.ToListAsync();

      if (!string.IsNullOrEmpty(searchManufacturerTypeName) && ManufacturerTypes.Count == 0)
      {
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "No Manufacturer Type found with the name '" + searchManufacturerTypeName + "'. Please check the name and try again.");
      }

      return View("~/Views/StoreManagement/MasterInfo/ManufacturerType/ManufacturerType.cshtml", ManufacturerTypes);
    }
    public async Task<IActionResult> ManufacturerType()
    {
      var ManufacturerTypes = await _appDBContext.Settings_ManufacturerTypes.ToListAsync();
      return Ok(ManufacturerTypes);
    }// Add the Edit action
    public async Task<IActionResult> Edit(int id)
    {
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      var ManufacturerType = await _appDBContext.Settings_ManufacturerTypes.FindAsync(id);
      if (ManufacturerType == null)
      {
        return NotFound();
      }
      return PartialView("~/Views/StoreManagement/MasterInfo/ManufacturerType/EditManufacturerType.cshtml", ManufacturerType);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Settings_ManufacturerType ManufacturerType)
    {
      if (ModelState.IsValid)
      {
        if (string.IsNullOrEmpty(ManufacturerType.ManufacturerTypeName))
        {
          return Json(new { success = false, message = "ManufacturerType Name field is required. Please enter a valid text value." });
        }
        _appDBContext.Update(ManufacturerType);
        await _appDBContext.SaveChangesAsync();
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Manufacturer Type Updated successfully.");
        return Json(new { success = true });
      }
      return Json(new { success = false, message = "Error creating ManufacturerType. Please check the inputs." });
    }
    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      return PartialView("~/Views/StoreManagement/MasterInfo/ManufacturerType/AddManufacturerType.cshtml", new Settings_ManufacturerType());
    }

    [HttpPost]
    public async Task<IActionResult> Create(Settings_ManufacturerType ManufacturerType)
    {
      if (ModelState.IsValid)
      {
        if (string.IsNullOrEmpty(ManufacturerType.ManufacturerTypeName))
        {
          return Json(new { success = false, message = "ManufacturerType Name field is required. Please enter a valid text value." });
        }
        ManufacturerType.DeleteYNID = 0;
        _appDBContext.Settings_ManufacturerTypes.Add(ManufacturerType);
        await _appDBContext.SaveChangesAsync();
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Manufacturer Type Created successfully.");
        return Json(new { success = true });
      }
      return Json(new { success = false, message = "Error creating ManufacturerType. Please check the inputs." });
    }
    public async Task<IActionResult> Delete(int id)
    {
      var ManufacturerType = await _appDBContext.Settings_ManufacturerTypes.FindAsync(id);
      if (ManufacturerType == null)
      {
        return NotFound();
      }

      ManufacturerType.ActiveYNID = 2;
      ManufacturerType.DeleteYNID = 1;

      _appDBContext.Settings_ManufacturerTypes.Update(ManufacturerType);
      await _appDBContext.SaveChangesAsync();
      await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Manufacturer Type Deleted successfully.");
      return Json(new { success = true });
    }
    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var ManufacturerTypees = await _appDBContext.Settings_ManufacturerTypes
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();

      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add("ManufacturerTypees");
        worksheet.Cells["A1"].Value = "ManufacturerType ID";
        worksheet.Cells["B1"].Value = "ManufacturerType Name";
        worksheet.Cells["C1"].Value = "Active";


        for (int i = 0; i < ManufacturerTypees.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = ManufacturerTypees[i].ManufacturerTypeID;
          worksheet.Cells[i + 2, 2].Value = ManufacturerTypees[i].ManufacturerTypeName;
          worksheet.Cells[i + 2, 3].Value = ManufacturerTypees[i].ActiveYNID == 1 ? "Yes" : "No";
        }

        worksheet.Cells["A1:C1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = $"ManufacturerTypees-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
    public async Task<IActionResult> Print()
    {
      var ManufacturerTypees = await _appDBContext.Settings_ManufacturerTypes
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();
      return View("~/Views/StoreManagement/MasterInfo/ManufacturerType/PrintManufacturerType.cshtml", ManufacturerTypees);
    }

  }
}

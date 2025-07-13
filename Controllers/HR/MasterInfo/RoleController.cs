using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;
using OfficeOpenXml;
using Microsoft.Extensions.Localization;

namespace Exampler_ERP.Controllers.HR.MasterInfo
{
  public class RoleController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IStringLocalizer<RoleController> _localizer;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    private readonly IHubContext<NotificationHub> _hubContext;


    public RoleController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext, IStringLocalizer<RoleController> localizer)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
      _hubContext = hubContext;
      _localizer = localizer;

    }
    public async Task<IActionResult> Index(string searchRoleTypeName)
    {

      var RoleTypesQuery = _appDBContext.Settings_RoleTypes
          .Where(b => b.DeleteYNID != 1);

      if (!string.IsNullOrEmpty(searchRoleTypeName))
      {
        RoleTypesQuery = RoleTypesQuery.Where(b => b.RoleTypeName.Contains(searchRoleTypeName));
      }

      var RoleTypes = await RoleTypesQuery.ToListAsync();

      if (!string.IsNullOrEmpty(searchRoleTypeName) && RoleTypes.Count == 0)
      {
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "No Role found with the name '" + searchRoleTypeName + "'. Please check the name and try again.");
      }

      return View("~/Views/HR/MasterInfo/Role/Role.cshtml", RoleTypes);
    }

    public async Task<IActionResult> Edit(int id)
    {
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      var Role = await _appDBContext.Settings_RoleTypes.FindAsync(id);
      if (Role == null)
      {
        return NotFound();
      }
      return PartialView("~/Views/HR/MasterInfo/Role/EditRole.cshtml", Role);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Settings_RoleType Role)
    {
      if (ModelState.IsValid)
      {
        if (string.IsNullOrEmpty(Role.RoleTypeName))
        {
          return Json(new { success = false, message = "Role Name field is required. Please enter a valid text value." });
        }
        _appDBContext.Update(Role);
        await _appDBContext.SaveChangesAsync();
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Role Updated successfully.");
        return Json(new { success = true });
      }
      return Json(new { success = false, message = "Error creating Role. Please check the inputs." });
    }
    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      return PartialView("~/Views/HR/MasterInfo/Role/AddRole.cshtml", new Settings_RoleType());
    }

    [HttpPost]
    public async Task<IActionResult> Create(Settings_RoleType Role)
    {
      if (ModelState.IsValid)
      {
        if (string.IsNullOrEmpty(Role.RoleTypeName))
        {
          return Json(new { success = false, message = "Role Name field is required. Please enter a valid text value." });
        }
        Role.DeleteYNID = 0;
        _appDBContext.Settings_RoleTypes.Add(Role);
        await _appDBContext.SaveChangesAsync();
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Role Created successfully.");
        return Json(new { success = true });
      }
      return Json(new { success = false, message = "Error creating Role. Please check the inputs." });
    }
    public async Task<IActionResult> Delete(int id)
    {
      var Role = await _appDBContext.Settings_RoleTypes.FindAsync(id);
      if (Role == null)
      {
        return NotFound();
      }

      Role.ActiveYNID = 2;
      Role.DeleteYNID = 1;

      _appDBContext.Settings_RoleTypes.Update(Role);
      await _appDBContext.SaveChangesAsync();
      await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Role Deleted successfully.");
      return Json(new { success = true });
    }
    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var Rolees = await _appDBContext.Settings_RoleTypes
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();

      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add("Rolees");
        worksheet.Cells["A1"].Value = "Role ID";
        worksheet.Cells["B1"].Value = "Role Name";
        worksheet.Cells["C1"].Value = "Active";


        for (int i = 0; i < Rolees.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = Rolees[i].RoleTypeID;
          worksheet.Cells[i + 2, 2].Value = Rolees[i].RoleTypeName;
          worksheet.Cells[i + 2, 3].Value = Rolees[i].ActiveYNID == 1 ? "Yes" : "No";
        }

        worksheet.Cells["A1:C1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = $"Rolees-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
    public async Task<IActionResult> Print()
    {
      var Rolees = await _appDBContext.Settings_RoleTypes
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();
      return View("~/Views/HR/MasterInfo/Role/PrintRoles.cshtml", Rolees);
    }

  }
}

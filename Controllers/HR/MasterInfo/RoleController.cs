using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace Exampler_ERP.Controllers.HR.MasterInfo
{
  public class RoleController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;

    public RoleController(AppDBContext appDBContext, IConfiguration configuration, Utils utils)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
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
        _appDBContext.Update(Role);
        await _appDBContext.SaveChangesAsync();
        return Json(new { success = true });
      }
      return PartialView("~/Views/HR/MasterInfo/Role/EditRole.cshtml", Role);
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
        Role.DeleteYNID = 0;
        _appDBContext.Settings_RoleTypes.Add(Role);
        await _appDBContext.SaveChangesAsync();
        return Json(new { success = true });
      }
      return PartialView("~/Views/HR/MasterInfo/Role/AddRole.cshtml", Role);
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

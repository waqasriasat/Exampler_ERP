using Azure;
using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;
using OfficeOpenXml;
using System.Data;
using System.Drawing.Printing;
using Microsoft.Extensions.Localization;

namespace Exampler_ERP.Controllers.HR.MasterInfo
{
  public class DepartmentController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IStringLocalizer<DepartmentController> _localizer;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    private readonly IHubContext<NotificationHub> _hubContext;


    public DepartmentController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext, IStringLocalizer<DepartmentController> localizer)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
      _hubContext = hubContext;
      _localizer = localizer;

    }
    public async Task<IActionResult> Index(string searchDepartmentName)
    {
      var departmentsQuery = _appDBContext.Settings_DepartmentTypes
          .Where(d => d.DeleteYNID != 1);

      if (!string.IsNullOrEmpty(searchDepartmentName))
      {
        departmentsQuery = departmentsQuery.Where(d => d.DepartmentTypeName.Contains(searchDepartmentName));
      }

      var departments = await departmentsQuery
          .Include(d => d.BranchType).ToListAsync();


      if (!string.IsNullOrEmpty(searchDepartmentName) && departments.Count == 0)
      {
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "No Department found with the name '" + searchDepartmentName + "'. Please check the name and try again.");
      }

      return View("~/Views/HR/MasterInfo/Department/Department.cshtml", departments);
    }


    public async Task<IActionResult> Department()
    {
      var Departments = await _appDBContext.Settings_DepartmentTypes.ToListAsync();
      return Ok(Departments);
    }// Add the Edit action
    public async Task<IActionResult> Edit(int id)
    {
      ViewBag.BranchList = await _utils.GetBranchs();
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      var Department = await _appDBContext.Settings_DepartmentTypes.FindAsync(id);
      if (Department == null)
      {
        return NotFound();
      }
      return PartialView("~/Views/HR/MasterInfo/Department/EditDepartment.cshtml", Department);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Settings_DepartmentType Department)
    {
      if (ModelState.IsValid)
      {
        if (string.IsNullOrEmpty(Department.DepartmentTypeName))
        {
          return Json(new { success = false, message = "Department Name field is required. Please enter a valid text value." });
        }

        _appDBContext.Update(Department);
        await _appDBContext.SaveChangesAsync();
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Department Updated successfully.");
        return Json(new { success = true });
      }
      return Json(new { success = false, message = "Error creating Department. Please check the inputs." });
    }
    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.BranchList = await _utils.GetBranchs();
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      return PartialView("~/Views/HR/MasterInfo/Department/AddDepartment.cshtml", new Settings_DepartmentType());
    }

    [HttpPost]
    public async Task<IActionResult> Create(Settings_DepartmentType Department)
    {
      if (ModelState.IsValid)
      {

        if (string.IsNullOrEmpty(Department.DepartmentTypeName))
        {
          return Json(new { success = false, message = "Department Name field is required. Please enter a valid text value." });
        }

        Department.DeleteYNID = 0;
        _appDBContext.Settings_DepartmentTypes.Add(Department);
        await _appDBContext.SaveChangesAsync();
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Department Created successfully.");
        return Json(new { success = true });
      }
      return Json(new { success = false, message = "Error creating Department. Please check the inputs." });
    }
    public async Task<IActionResult> Delete(int id)
    {
      var Department = await _appDBContext.Settings_DepartmentTypes.FindAsync(id);
      if (Department == null)
      {
        return NotFound();
      }

      Department.ActiveYNID = 2;
      Department.DeleteYNID = 1;

      _appDBContext.Settings_DepartmentTypes.Update(Department);
      await _appDBContext.SaveChangesAsync();
      await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Department Deleted successfully.");

      return Json(new { success = true });
    }
    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var Departments = await _appDBContext.Settings_DepartmentTypes
        .Where(b => b.DeleteYNID != 1)
        .Include(d => d.BranchType) // Eagerly load the related Branch data
        .ToListAsync();


      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add("Departments");
        worksheet.Cells["A1"].Value = "Department ID";
        worksheet.Cells["B1"].Value = _localizer["lbl_BranchName"];
        worksheet.Cells["C1"].Value = "Department Name";
        worksheet.Cells["D1"].Value = _localizer["lbl_Active"];


        for (int i = 0; i < Departments.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = Departments[i].DepartmentTypeID;
          worksheet.Cells[i + 2, 2].Value = Departments[i].BranchType?.BranchTypeName;
          worksheet.Cells[i + 2, 3].Value = Departments[i].DepartmentTypeName;
          worksheet.Cells[i + 2, 4].Value = Departments[i].ActiveYNID == 1 ? "Yes" : "No";
        }

        worksheet.Cells["A1:l1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = $"Departments-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
    public async Task<IActionResult> Print()
    {
      var Departments = await _appDBContext.Settings_DepartmentTypes
        .Where(b => b.DeleteYNID != 1)
        .Include(d => d.BranchType) // Eagerly load the related Branch data
        .ToListAsync();
      return View("~/Views/HR/MasterInfo/Department/PrintDepartments.cshtml", Departments);
    }

  }
}

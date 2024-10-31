using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace Exampler_ERP.Controllers.HR.HR
{
  public class DeductionController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;

    public DeductionController(AppDBContext appDBContext, IConfiguration configuration, Utils utils)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
    }
   
    public async Task<IActionResult> Index(int? MonthsTypeID, int? YearsTypeID, string? EmployeeName, int? EmployeeID, int? DeducationTypeID)
    {
      var query = _appDBContext.HR_Deductions
          .Where(b => b.DeleteYNID != 1)
          .Include(d => d.Employee)
          .Include(d => d.DeductionType)
          .AsQueryable();

      if (MonthsTypeID.HasValue && MonthsTypeID != 0)
      {
        query = query.Where(d => d.Month == MonthsTypeID.Value);
      }

      if (YearsTypeID.HasValue)
      {
        query = query.Where(d => d.Year == YearsTypeID.Value);
      }

      if (EmployeeID.HasValue)
      {
        query = query.Where(d => d.EmployeeID == EmployeeID.Value);
      }

      if (!string.IsNullOrEmpty(EmployeeName))
      {
        query = query.Where(d =>
            (d.Employee.FirstName + " " + d.Employee.FatherName + " " + d.Employee.FamilyName)
            .Contains(EmployeeName));
      }

      if (DeducationTypeID.HasValue && DeducationTypeID != 0)
      {
        query = query.Where(d => d.DeductionTypeID == DeducationTypeID.Value);
      }

      var deductions = await query.ToListAsync();

      ViewBag.MonthsTypeID = MonthsTypeID;
      ViewBag.YearsTypeID = YearsTypeID;
      ViewBag.DeducationTypeID = DeducationTypeID;
      ViewBag.EmployeeID = EmployeeID;
      ViewBag.EmployeeName = EmployeeName;

      ViewBag.MonthsTypeList = await _utils.GetMonthsTypes();
      ViewBag.DeducationTypeList = await _utils.GetDeductionTypes();

      return View("~/Views/HR/HR/Deduction/Deduction.cshtml", deductions);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
      // Load necessary data for dropdowns or lists
      ViewBag.EmployeesList = await _utils.GetEmployee();
      ViewBag.DeductionTypesList = await _utils.GetDeductionTypes();

      // Find the deduction by ID
      var deduction = await _appDBContext.HR_Deductions
                                         .Include(d => d.Employee)
                                         .Include(d => d.DeductionType)
                                         .FirstOrDefaultAsync(d => d.DeductionID == id && d.DeleteYNID != 1 && (d.PostedID == 0 || d.PostedID == null));

      if (deduction == null)
      {
        return NotFound();
      }

      // Return the partial view with the deduction model
      return PartialView("~/Views/HR/HR/Deduction/EditDeduction.cshtml", deduction);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(HR_Deduction deduction)
    {
      if (ModelState.IsValid)
      {
        try
        {
          _appDBContext.HR_Deductions.Update(deduction);
          await _appDBContext.SaveChangesAsync();
          TempData["SuccessMessage"] = "Deduction updated successfully.";
          return Json(new { success = true });
        }
        catch (Exception ex)
        {
          TempData["ErrorMessage"] = "Unable to save changes: " + ex.Message;
        }
      }

      // If model state is invalid, reload dropdowns or lists
      ViewBag.EmployeeList = await _utils.GetEmployee();
      ViewBag.DeductionTypesList = await _utils.GetDeductionTypes();

      // Return the partial view with validation errors
      TempData["ErrorMessage"] = "Error updating Deduction. Please check the inputs.";
      return PartialView("~/Views/HR/HR/Deduction/EditDeduction.cshtml", deduction);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.DeductionTypesList = await _utils.GetDeductionTypes();
      ViewBag.EmployeesList = await _utils.GetEmployee();
      return PartialView("~/Views/HR/HR/Deduction/AddDeduction.cshtml", new HR_Deduction());
    }

    [HttpPost]
    public async Task<IActionResult> Create(HR_Deduction Deduction)
    {
      if (ModelState.IsValid)
      {
        Deduction.DeleteYNID = 0;
        _appDBContext.HR_Deductions.Add(Deduction);
        await _appDBContext.SaveChangesAsync();
        TempData["SuccessMessage"] = "Deduction created successfully.";
        return Json(new { success = true });
      }
      TempData["ErrorMessage"] = "Error creating Deduction. Please check the inputs.";
      return PartialView("~/Views/HR/HR/Deduction/AddDeduction.cshtml", Deduction);
    }
    public async Task<IActionResult> Delete(int id)
    {
      var Deduction = await _appDBContext.HR_Deductions.FindAsync(id);
      if (Deduction == null)
      {
        return NotFound();
      }

      Deduction.DeleteYNID = 1;

      _appDBContext.HR_Deductions.Update(Deduction);
      await _appDBContext.SaveChangesAsync();
      TempData["SuccessMessage"] = "Deduction deleted successfully.";
      return Json(new { success = true });
    }
    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var Deduction = await _appDBContext.HR_Deductions
        .Where(b => b.DeleteYNID != 1)
        .Include(d => d.Employee)
        .Include(d => d.DeductionType)
        .ToListAsync();


      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add("Deduction");
        worksheet.Cells["A1"].Value = "Deduction ID";
        worksheet.Cells["B1"].Value = "Employee Name";
        worksheet.Cells["C1"].Value = "Deduction Type";
        worksheet.Cells["D1"].Value = "Month";
        worksheet.Cells["E1"].Value = "Year";
        worksheet.Cells["F1"].Value = "Days";


        for (int i = 0; i < Deduction.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = Deduction[i].DeductionID;
          worksheet.Cells[i + 2, 2].Value = Deduction[i].Employee?.FirstName +' '+ Deduction[i].Employee?.FatherName+' '+ Deduction[i].Employee?.FamilyName;
          worksheet.Cells[i + 2, 3].Value = Deduction[i].DeductionType?.DeductionTypeName;
          worksheet.Cells[i + 2, 4].Value = Deduction[i].Month;
          worksheet.Cells[i + 2, 5].Value = Deduction[i].Year;
          worksheet.Cells[i + 2, 6].Value = Deduction[i].Days;
        }

        worksheet.Cells["A1:l1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = $"Deduction-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
    public async Task<IActionResult> Print()
    {
      var Deduction = await _appDBContext.HR_Deductions
        .Where(b => b.DeleteYNID != 1)
        .Include(d => d.Employee)
        .Include(d => d.DeductionType)
        .ToListAsync();
      return View("~/Views/HR/HR/Deduction/PrintDeduction.cshtml", Deduction);
    }
  }
}

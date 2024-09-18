using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace Exampler_ERP.Controllers.HR.Financial
{
  public class OverTimeController : Controller
 
    {
      private readonly AppDBContext _appDBContext;
      private readonly IConfiguration _configuration;
      private readonly Utils _utils;

      public OverTimeController(AppDBContext appDBContext, IConfiguration configuration, Utils utils)
      {
        _appDBContext = appDBContext;
        _configuration = configuration;
        _utils = utils;
      }
      public async Task<IActionResult> Index()
      {
        var OverTimes = await _appDBContext.HR_OverTimes
          .Where(b => b.DeleteYNID != 1)
          .Include(d => d.Employee)
          .ToListAsync();
        return View("~/Views/HR/Financial/OverTime/OverTime.cshtml", OverTimes);
      }
      [HttpGet]
      public async Task<IActionResult> Edit(int id)
      {
        // Load necessary data for dropdowns or lists
        ViewBag.EmployeesList = await _utils.GetEmployee();

        // Find the OverTime by ID
        var OverTime = await _appDBContext.HR_OverTimes
                                           .Include(d => d.Employee)
                                           .FirstOrDefaultAsync(d => d.OverTimeID == id && d.DeleteYNID != 1);

        if (OverTime == null)
        {
          return NotFound();
        }

        // Return the partial view with the OverTime model
        return PartialView("~/Views/HR/Financial/OverTime/EditOverTime.cshtml", OverTime);
      }

      [HttpPost]
      public async Task<IActionResult> Edit(HR_OverTime OverTime)
      {
        if (ModelState.IsValid)
        {
          try
          {
            _appDBContext.HR_OverTimes.Update(OverTime);
            await _appDBContext.SaveChangesAsync();
            return Json(new { success = true });
          }
          catch (Exception ex)
          {
            ModelState.AddModelError("", "Unable to save changes: " + ex.Message);
          }
        }

        // If model state is invalid, reload dropdowns or lists
        ViewBag.EmployeeList = await _utils.GetEmployee();

        // Return the partial view with validation errors
        return PartialView("~/Views/HR/Financial/OverTime/EditOverTime.cshtml", OverTime);
      }

      [HttpGet]
      public async Task<IActionResult> Create()
      {
        ViewBag.EmployeesList = await _utils.GetEmployee();
        return PartialView("~/Views/HR/Financial/OverTime/AddOverTime.cshtml", new HR_OverTime());
      }

      [HttpPost]
      public async Task<IActionResult> Create(HR_OverTime OverTime)
      {
        if (ModelState.IsValid)
        {
          OverTime.DeleteYNID = 0;
          _appDBContext.HR_OverTimes.Add(OverTime);
          await _appDBContext.SaveChangesAsync();
          return Json(new { success = true });
        }
        return PartialView("~/Views/HR/Financial/OverTime/AddOverTime.cshtml", OverTime);
      }
      public async Task<IActionResult> Delete(int id)
      {
        var OverTime = await _appDBContext.HR_OverTimes.FindAsync(id);
        if (OverTime == null)
        {
          return NotFound();
        }

        OverTime.DeleteYNID = 1;

        _appDBContext.HR_OverTimes.Update(OverTime);
        await _appDBContext.SaveChangesAsync();

        return Json(new { success = true });
      }
      public async Task<IActionResult> ExportToExcel()
      {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        var OverTime = await _appDBContext.HR_OverTimes
          .Where(b => b.DeleteYNID != 1)
          .Include(d => d.Employee)
          .ToListAsync();


        using (var package = new ExcelPackage())
        {
          var worksheet = package.Workbook.Worksheets.Add("OverTime");
          worksheet.Cells["A1"].Value = "OverTime ID";
          worksheet.Cells["B1"].Value = "Employee Name";
          worksheet.Cells["C1"].Value = "Month";
          worksheet.Cells["D1"].Value = "Year";
          worksheet.Cells["E1"].Value = "Amount";


          for (int i = 0; i < OverTime.Count; i++)
          {
            worksheet.Cells[i + 2, 1].Value = OverTime[i].OverTimeID;
            worksheet.Cells[i + 2, 2].Value = OverTime[i].Employee?.FirstName + ' ' + OverTime[i].Employee?.FatherName + ' ' + OverTime[i].Employee?.FamilyName;
            worksheet.Cells[i + 2, 3].Value = OverTime[i].Month;
            worksheet.Cells[i + 2, 4].Value = OverTime[i].Year;
            worksheet.Cells[i + 2, 5].Value = OverTime[i].Amount;
          }

          worksheet.Cells["A1:l1"].Style.Font.Bold = true;
          worksheet.Cells.AutoFitColumns();

          var stream = new MemoryStream();
          package.SaveAs(stream);
          stream.Position = 0;
          string excelName = $"OverTime-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

          return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }
      }
      public async Task<IActionResult> Print()
      {
        var OverTime = await _appDBContext.HR_OverTimes
          .Where(b => b.DeleteYNID != 1)
          .Include(d => d.Employee)
          .ToListAsync();
        return View("~/Views/HR/Financial/OverTime/PrintOverTime.cshtml", OverTime);
      }
    }
  }

using Exampler_ERP.Models;
using Exampler_ERP.Models.Temp;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace Exampler_ERP.Controllers.HR.HR
{
  public class HolidayController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;

    public HolidayController(AppDBContext appDBContext, IConfiguration configuration, Utils utils)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
    }

    public async Task<IActionResult> Index(int? MonthsTypeID, int? YearsTypeID, string? HolidayTypeName, int? HolidayTypeID)
    {
      var HolidayQuery = _appDBContext.HR_Holidays
          .Where(c => c.DeleteYNID != 1);

      if (HolidayTypeID.HasValue && HolidayTypeID > 0)
      {
        HolidayQuery = HolidayQuery.Where(c => c.HolidayTypeID == HolidayTypeID.Value);
      }

      if (MonthsTypeID.HasValue && YearsTypeID.HasValue)
      {
        HolidayQuery = HolidayQuery.Where(c => c.HolidayDate.Month == MonthsTypeID.Value && c.HolidayDate.Year == YearsTypeID.Value);
      }

      var Holidays = await HolidayQuery
          .Include(c => c.HolidayType)
          .ToListAsync();

      if (!Holidays.Any())
      {
        TempData["ErrorMessage"] = "No Holidays Found for the selected filters.";
      }

      ViewBag.MonthsTypeID = MonthsTypeID;
      ViewBag.YearsTypeID = YearsTypeID;
      ViewBag.HolidayTypeID = HolidayTypeID;

      ViewBag.MonthsTypeList = await _utils.GetMonthsTypes();
      ViewBag.HolidayTypeList = await _utils.GetHolidayTypes();

      return View("~/Views/HR/HR/Holiday/Holiday.cshtml", Holidays);
    }


    public async Task<IActionResult> Holiday()
    {
      var Holidays = await _appDBContext.HR_Holidays.ToListAsync();
      return Ok(Holidays);
    }

    public async Task<IActionResult> Edit(int id)
    {
      var Holiday = await _appDBContext.HR_Holidays.FindAsync(id);
      if (Holiday == null)
      {
        return NotFound();
      }

      ViewBag.HolidayTypesList = await _utils.GetHolidayTypes();

      return PartialView("~/Views/HR/HR/Holiday/EditHoliday.cshtml", Holiday);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(HR_Holiday Holiday)
    {
      if (ModelState.IsValid)
      {

        _appDBContext.Update(Holiday);
        await _appDBContext.SaveChangesAsync();
        TempData["SuccessMessage"] = "Holiday Updated successfully.";
        return Json(new { success = true });
      }
      return Json(new { success = false, message = "Error creating Holiday. Please check the inputs." });
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.HolidayTypesList = await _utils.GetHolidayTypes();
      return PartialView("~/Views/HR/HR/Holiday/AddHoliday.cshtml", new HR_Holiday());
    }

    [HttpPost]
    public async Task<IActionResult> Create(HR_Holiday Holiday)
    {
      if (ModelState.IsValid)
      {

        Holiday.DeleteYNID = 0;
        Holiday.ActiveYNID = 2;
        Holiday.FinalApprovalID = 0;
        _appDBContext.HR_Holidays.Add(Holiday);
        await _appDBContext.SaveChangesAsync();

        var HolidayId = Holiday.HolidayID;
        if (HolidayId > 0)
        {
          var processCount = await _appDBContext.CR_ProcessTypeApprovalSetups
                              .Where(pta => pta.ProcessTypeID > 0 && pta.ProcessTypeID == 15)
                              .CountAsync();

          if (processCount > 0)
          {
            var newProcessTypeApproval = new CR_ProcessTypeApproval
            {
              ProcessTypeID = 15,
              Notes = "Holiday Approval",
              Date = DateTime.Now,
              EmployeeID = HttpContext.Session.GetInt32("UserID") ?? default(int),
              UserID = HttpContext.Session.GetInt32("UserID") ?? default(int),
              TransactionID = Holiday.HolidayID
            };

            _appDBContext.CR_ProcessTypeApprovals.Add(newProcessTypeApproval);
            await _appDBContext.SaveChangesAsync();

            var nextApprovalSetup = await _appDBContext.CR_ProcessTypeApprovalSetups
                                        .Where(pta => pta.ProcessTypeID == 15 && pta.Rank == 1)
                                        .FirstOrDefaultAsync();

            if (nextApprovalSetup != null)
            {
              var newProcessTypeApprovalDetail = new CR_ProcessTypeApprovalDetail
              {
                ProcessTypeApprovalID = newProcessTypeApproval.ProcessTypeApprovalID,
                Date = DateTime.Now,
                RoleID = nextApprovalSetup.RoleTypeID,
                AppID = 0,
                AppUserID = 0,
                Notes = null,
                Rank = nextApprovalSetup.Rank
              };

              _appDBContext.CR_ProcessTypeApprovalDetails.Add(newProcessTypeApprovalDetail);
              await _appDBContext.SaveChangesAsync();
            }
            else
            {
              return Json(new { success = false, message = "Next approval setup not found." });
            }
          }
          else
          {
            Holiday.ActiveYNID = 1;
            Holiday.FinalApprovalID = 1;
            _appDBContext.HR_Holidays.Update(Holiday);
            await _appDBContext.SaveChangesAsync();
            TempData["SuccessMessage"] = "Holiday Created successfully. No process setup found, Holiday activated.";
            return Json(new { success = true, message = "No process setup found, Holiday activated." });
          }
        }
        TempData["SuccessMessage"] = "Holiday Created successfully. Continue to the Approval Process Setup for Holiday Activation.";
        return Json(new { success = true });
      }
      return Json(new { success = false, message = "Error creating Employee. Please check the inputs." });
    }

    public async Task<IActionResult> Delete(int id)
    {
      var Holiday = await _appDBContext.HR_Holidays.FindAsync(id);
      if (Holiday == null)
      {
        return NotFound();
      }

      Holiday.ActiveYNID = 2;
      Holiday.DeleteYNID = 1;

      _appDBContext.HR_Holidays.Update(Holiday);
      await _appDBContext.SaveChangesAsync();
      TempData["SuccessMessage"] = "Holiday Deleted successfully.";
      return Json(new { success = true });
    }

    public async Task<IActionResult> Print()
    {
      var Holidays = await _appDBContext.HR_Holidays
          .Where(c => c.DeleteYNID != 1)
          .Include(c => c.HolidayType)
          .ToListAsync();

      return View("~/Views/HR/HR/Holiday/PrintHolidays.cshtml", Holidays);
    }

    //public async Task<IActionResult> ExportToExcel()
    //{
    //  ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

    //  var Holidays = await _appDBContext.HR_Holidays
    //      .Where(c => c.DeleteYNID != 1)
    //      .Include(c => c.HolidayType)
    //      .ToListAsync();
    //  var SalaryTypesList = await _utils.GetSalaryOptions();
    //  using (var package = new ExcelPackage())
    //  {
    //    var worksheet = package.Workbook.Worksheets.Add("Holidays");

    //    worksheet.Cells["A1"].Value = "Holiday ID";
    //    worksheet.Cells["B1"].Value = "Employee Name";
    //    worksheet.Cells["C1"].Value = "Issue Date";
    //    worksheet.Cells["D1"].Value = "Salary Type";
    //    worksheet.Cells["E1"].Value = "Holiday Type";
    //    worksheet.Cells["F1"].Value = "Vacation Days";
    //    worksheet.Cells["G1"].Value = "Daily Hours";
    //    worksheet.Cells["H1"].Value = "Daily Minutes";
    //    worksheet.Cells["I1"].Value = "Final Approval ID";
    //    worksheet.Cells["J1"].Value = "Approval Process ID";

    //    for (int i = 0; i < Holidays.Count; i++)
    //    {
    //      worksheet.Cells[i + 2, 1].Value = Holidays[i].HolidayID;
    //      worksheet.Cells[i + 2, 2].Value = Holidays[i].Employee?.FirstName + ' ' + Holidays[i].Employee?.FatherName + ' ' + Holidays[i].Employee?.FamilyName;
    //      worksheet.Cells[i + 2, 3].Value = Holidays[i].IssueDate.ToString("dd-MMM-yyyy");
    //      worksheet.Cells[i + 2, 4].Value = Holidays[i].SalaryTypeID == 0 || Holidays[i]?.SalaryTypeID == null
    //      ? ""
    //      : SalaryTypesList.FirstOrDefault(g => g.Value == Holidays[i].SalaryTypeID.ToString())?.Text;
    //      worksheet.Cells[i + 2, 5].Value = Holidays[i].HolidayTypeID;
    //      worksheet.Cells[i + 2, 6].Value = Holidays[i].VacationDays;
    //      worksheet.Cells[i + 2, 7].Value = Holidays[i].DHours;
    //      worksheet.Cells[i + 2, 8].Value = Holidays[i].DMinutes;
    //      worksheet.Cells[i + 2, 9].Value = Holidays[i].FinalApprovalID;
    //      worksheet.Cells[i + 2, 10].Value = Holidays[i].ProcessTypeApprovalID;
    //    }

    //    worksheet.Cells["A1:J1"].Style.Font.Bold = true;
    //    worksheet.Cells.AutoFitColumns();

    //    var stream = new MemoryStream();
    //    package.SaveAs(stream);
    //    stream.Position = 0;
    //    string excelName = $"Holidays-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

    //    return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
    //  }
    //}
  }
}

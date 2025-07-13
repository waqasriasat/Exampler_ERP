using Exampler_ERP.Models;
using Exampler_ERP.Models.Temp;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OfficeOpenXml;
using System.Data;
using Microsoft.Extensions.Localization;

namespace Exampler_ERP.Controllers.HR.HR
{
  public class FaceAttendanceForwardingController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IStringLocalizer<FaceAttendanceForwardingController> _localizer;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    private readonly IHubContext<NotificationHub> _hubContext;


    public FaceAttendanceForwardingController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext, IStringLocalizer<FaceAttendanceForwardingController> localizer)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
      _hubContext = hubContext;
      _localizer = localizer;

    }
    public async Task<IActionResult> Index(int? Branch, int? MonthsTypeID, int? YearsTypeID)
    {


      HttpContext.Session.SetInt32("FaceAttendanceBranchID", Branch ?? 0);
      HttpContext.Session.SetInt32("FaceAttendanceMonthID", MonthsTypeID ?? 0);
      HttpContext.Session.SetInt32("FaceAttendanceYearID", YearsTypeID ?? 0);

      var attendanceRecords = new List<FaceAttendanceForwardingViewModel>();
      var connectionString = _configuration.GetConnectionString("AppDb");
      using (SqlConnection connection = new SqlConnection(connectionString))
      {
        using (SqlCommand command = new SqlCommand("GetFaceAttendanceForwarding", connection))
        {
          command.CommandType = CommandType.StoredProcedure;
          command.Parameters.AddWithValue("@MonthID", MonthsTypeID ?? 0); // Default to October if null
          command.Parameters.AddWithValue("@YearID", YearsTypeID ?? 0); // Default to 2024 if null
          command.Parameters.AddWithValue("@BranchID", Branch ?? 0); // Set your branch ID accordingly

          connection.Open();
          using (SqlDataReader reader = await command.ExecuteReaderAsync())
          {
            while (await reader.ReadAsync())
            {
              var record = new FaceAttendanceForwardingViewModel();
              try
              {
                // Wrap each field assignment in try-catch to isolate errors
                try { record.EmployeeID = reader.GetInt32(0); } catch (Exception ex) { Console.WriteLine("EmployeeID Error: " + ex.Message); }
                record.Employee = await _appDBContext.HR_Employees.FindAsync(record.EmployeeID);
                try { record.MarkDate = reader.GetDateTime(1); } catch (Exception ex) { Console.WriteLine("MarkDate Error: " + ex.Message); }
                try { record.InTime = reader.GetString(2); } catch (Exception ex) { Console.WriteLine("InTime Error: " + ex.Message); }
                try { record.OutTime = reader.GetString(3); } catch (Exception ex) { Console.WriteLine("OutTime Error: " + ex.Message); }
                try { record.DHours = reader.GetInt32(4); } catch (Exception ex) { Console.WriteLine("DHours Error: " + ex.Message); }
                try { record.DMinutes = reader.GetInt32(5); } catch (Exception ex) { Console.WriteLine("DMinutes Error: " + ex.Message); }
                try { record.FromDutyTime = reader.GetString(6); } catch (Exception ex) { Console.WriteLine("FromDutyTime Error: " + ex.Message); }
                try { record.ToDutyTime = reader.GetString(7); } catch (Exception ex) { Console.WriteLine("ToDutyTime Error: " + ex.Message); }
                try { record.LateComingGraceTime = reader.GetDecimal(8); } catch (Exception ex) { Console.WriteLine("LateComingGraceTime Error: " + ex.Message); }
                try { record.EarlyGoingGraceTime = reader.GetDecimal(9); } catch (Exception ex) { Console.WriteLine("EarlyGoingGraceTime Error: " + ex.Message); }
                try { record.PerDaySalary = reader.GetString(10); } catch (Exception ex) { Console.WriteLine("PerDaySalary Error: " + ex.Message); }
                try { record.LateComingDeduction = reader.GetString(11); } catch (Exception ex) { Console.WriteLine("LateComingDeduction Error: " + ex.Message); }
                try { record.EarlyGoingDeduction = reader.GetString(12); } catch (Exception ex) { Console.WriteLine("EarlyGoingDeduction Error: " + ex.Message); }
                try { record.EarlyComingGraceTime = reader.GetDecimal(13); } catch (Exception ex) { Console.WriteLine("EarlyComingGraceTime Error: " + ex.Message); }
                try { record.LateGoingGraceTime = reader.GetDecimal(14); } catch (Exception ex) { Console.WriteLine("LateGoingGraceTime Error: " + ex.Message); }
                try { record.EarlyComingAmount = reader.GetString(15); } catch (Exception ex) { Console.WriteLine("PerDaySalary Error: " + ex.Message); }
                try { record.LateGoingAmount = reader.GetString(16); } catch (Exception ex) { Console.WriteLine("PerDaySalary Error: " + ex.Message); }

                attendanceRecords.Add(record);
              }
              catch (InvalidCastException ex)
              {
                await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "InvalidCastException occurred: " + ex.Message);
              }
            }
          }
        }
      }
      ViewBag.MonthsTypeID = MonthsTypeID;
      ViewBag.YearsTypeID = YearsTypeID;
      ViewBag.Branch = Branch;

      ViewBag.MonthsTypeList = await _utils.GetMonthsTypesWithoutZeroLine();
      ViewBag.BranchList = await _utils.GetBranchsWithoutZeroLine();
      return View("~/Views/HR/HR/FaceAttendanceForwarding/FaceAttendanceForwarding.cshtml", attendanceRecords);
    }

    private int CalculateWeekends(int daysInMonth, int monthID, int year, int employeeID)
    {
      int weekends = 0;
      for (int day = 1; day <= daysInMonth; day++)
      {
        DateTime date = new DateTime(year, monthID, day);
        if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
        {
          weekends++;
        }
      }
      return weekends;
    }
    [HttpPost]
    public async Task<JsonResult> UpdatePosted(List<EmployeeData> Employees)
    {
      try
      {
        var FatchExistingFaceAttendancePosted = await _appDBContext.CR_FaceAttendancePosteds
        .Where(e => e.PostedID == 1 && e.MonthTypeID == HttpContext.Session.GetInt32("FaceAttendanceMonthID") && e.Year == HttpContext.Session.GetInt32("FaceAttendanceYearID") && e.BranchTypeID == HttpContext.Session.GetInt32("FaceAttendanceBranchID"))
        .FirstOrDefaultAsync();



        if (FatchExistingFaceAttendancePosted != null)
        {
          await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "Already posting found for the specified branch, month, and year.");
          return Json(new { success = false });
        }

        var groupedEmployees = Employees.GroupBy(emp => emp.EmployeeID);

        foreach (var group in groupedEmployees)
        {
          var employeeID = group.Key;
          var employee = group.First();

          int monthID = employee.MarkDate.Month;
          int year = employee.MarkDate.Year;

          int daysInMonth = DateTime.DaysInMonth(year, monthID);
          int weekends = CalculateWeekends(daysInMonth, monthID, year, employee.EmployeeID);

          int holidays = _appDBContext.HR_Holidays
              .Where(h => h.HolidayDate.Month == monthID && h.HolidayDate.Year == year)
              .Count();

          int workingDaysInWeek = _appDBContext.HR_GlobalSettings
          .Select(gs => (int?)gs.WorkingDayInWeek) // Cast to nullable int
          .FirstOrDefault() ?? 5;

          int totalWorkingDays = daysInMonth - weekends - holidays;

          int presentDays = _appDBContext.CR_FaceAttendances
              .Where(a => a.EmployeeID == employee.EmployeeID && a.MarkDate.Month == monthID && a.MarkDate.Year == year)
              .Count();

          int absentDays = totalWorkingDays - presentDays;

          bool deductionExists = _appDBContext.HR_Deductions
              .Any(d => d.EmployeeID == employeeID && d.Month == monthID && d.Year == year && d.DeductionTypeID == 3);



          var perDaySalary = _appDBContext.HR_Salarys
              .Join(_appDBContext.HR_SalaryDetails,
              hs => hs.SalaryID,
              hrsd => hrsd.SalaryID,
              (hs, hrsd) => new
              {
                hs.EmployeeID,
                hrsd.SalaryAmount
              })
              .Where(s => s.EmployeeID == employeeID)
              .Select(s => new
              {
                PerDaySalary = (s.SalaryAmount /
                      (monthID == 2
                          ? (DateTime.IsLeapYear(year) ? 29.0 : 28.0)
                          : (new[] { 4, 6, 9, 11 }.Contains(monthID) ? 30.0 : 31.0)
                      )
                  )
              })
              .FirstOrDefault();

          float perDaySalarys = (float)(perDaySalary?.PerDaySalary.GetValueOrDefault(0) ?? 0);
          float amount = absentDays * perDaySalarys;
          amount = (float)Math.Round(amount, 2);

          if (absentDays > 0 && !deductionExists)
          {
            var deduction = new HR_Deduction
            {
              DeductionTypeID = 3, // Absence Deduction
              EmployeeID = employeeID,
              Month = monthID,
              Year = year,
              Days = absentDays,
              FromDate = new DateTime(year, monthID, 1),
              ToDate = new DateTime(year, monthID, daysInMonth),
              Amount = amount, // Calculated deduction amount
              DeleteYNID = 0,
              FinalApprovalID = 1
            };

            _appDBContext.HR_Deductions.Add(deduction);
          }
          if (absentDays < 0)
          {
            var addionalAllowance = _appDBContext.HR_AddionalAllowances
                .Where(a => a.EmployeeID == employeeID && a.MonthTypeID == monthID && a.Year == year)
                .FirstOrDefault();

            if (addionalAllowance != null)
            {
              var allowanceAmount = Math.Abs(absentDays) * perDaySalarys;
              var allowanceDetail = new HR_AddionalAllowanceDetail
              {
                AddionalAllowanceID = addionalAllowance.AddionalAllowanceID,
                AddionalAllowanceTypeID = 1,
                AddionalAllowanceAmount = allowanceAmount
              };

              _appDBContext.HR_AddionalAllowanceDetails.Add(allowanceDetail);
            }
            else
            {
              var newAllowance = new HR_AddionalAllowance
              {
                EmployeeID = employeeID,
                MonthTypeID = monthID,
                Year = year,
                FinalApprovalID = 1,
                ProcessTypeApprovalID = 1,
                PostedID = 0,
                PayRollID = 0
              };

              _appDBContext.HR_AddionalAllowances.Add(newAllowance);
              _appDBContext.SaveChanges();

              var allowanceAmount = Math.Abs(absentDays) * perDaySalarys;
              var allowanceDetail = new HR_AddionalAllowanceDetail
              {
                AddionalAllowanceID = newAllowance.AddionalAllowanceID,
                AddionalAllowanceTypeID = 3,
                AddionalAllowanceAmount = allowanceAmount
              };

              _appDBContext.HR_AddionalAllowanceDetails.Add(allowanceDetail);
            }
          }
        }


        foreach (var employee in Employees)
        {
          int monthID = employee.MarkDate.Month;
          int year = employee.MarkDate.Year;
          if (employee.LateComingDeduction > 0)
          {
            var deduction = new HR_Deduction
            {
              DeductionTypeID = 1,
              EmployeeID = employee.EmployeeID,
              Month = monthID,
              Year = year,
              Days = 1,
              FromDate = employee.MarkDate,
              ToDate = employee.MarkDate,
              Amount = employee.LateComingDeduction,
              DeleteYNID = 0,
              FinalApprovalID = 1
            };
            _appDBContext.HR_Deductions.Add(deduction);
          }

          if (employee.EarlyGoingDeduction > 0)
          {
            var deduction = new HR_Deduction
            {
              DeductionTypeID = 2,
              EmployeeID = employee.EmployeeID,
              Month = monthID,
              Year = year,
              Days = 1,
              FromDate = employee.MarkDate,
              ToDate = employee.MarkDate,
              Amount = employee.EarlyGoingDeduction,
              DeleteYNID = 0,
              FinalApprovalID = 1
            };
            _appDBContext.HR_Deductions.Add(deduction);
          }

          if (employee.EarlyComingAmount > 0)
          {
            var overtime = new HR_OverTime
            {
              EmployeeID = employee.EmployeeID,
              Amount = employee.EarlyComingAmount,
              OverTimeTypeID = 1,
              MonthTypeID = monthID,
              Year = year,
              Days = 1,
              Hours = employee.EarlyComingGraceTime,
              DeleteYNID = 0,
              FinalApprovalID = 1
            };
            _appDBContext.HR_OverTimes.Add(overtime);
          }

          if (employee.LateGoingAmount > 0)
          {
            var overtime = new HR_OverTime
            {
              EmployeeID = employee.EmployeeID,
              Amount = employee.LateGoingAmount,
              OverTimeTypeID = 1,
              MonthTypeID = monthID,
              Year = year,
              Days = 1,
              Hours = employee.LateGoingGraceTime,
              DeleteYNID = 0,
              FinalApprovalID = 1
            };
            _appDBContext.HR_OverTimes.Add(overtime);
          }
        }
        var faceAttendancePosted = new CR_FaceAttendancePosted
        {
          BranchTypeID = HttpContext.Session.GetInt32("FaceAttendanceBranchID") ?? 0,
          MonthTypeID = HttpContext.Session.GetInt32("FaceAttendanceMonthID") ?? 0,
          Year = HttpContext.Session.GetInt32("FaceAttendanceYearID") ?? 0,
          PostedID = 1
        };
        _appDBContext.CR_FaceAttendancePosteds.Add(faceAttendancePosted);
        await _appDBContext.SaveChangesAsync();
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Successfully Posted.");
        return Json(new { success = true });
      }
      catch (Exception ex)
      {
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "InvalidCastException occurred: " + ex.Message);
        return Json(new { success = false, message = ex.Message });
      }
    }

    // Model class for employee data
    public class EmployeeData
    {
      public int EmployeeID { get; set; }
      public DateTime MarkDate { get; set; }
      public float LateComingDeduction { get; set; }
      public float EarlyGoingDeduction { get; set; }
      public int EarlyComingGraceTime { get; set; }
      public float EarlyComingAmount { get; set; }
      public int LateGoingGraceTime { get; set; }
      public float LateGoingAmount { get; set; }
    }




    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var FaceAttendance = await _appDBContext.CR_FaceAttendances
        .Where(b => b.DeleteYNID != 1)
        .Include(d => d.Employee)
        .ToListAsync();


      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add("FaceAttendance");
        worksheet.Cells["A1"].Value = "FaceAttendance ID";
        worksheet.Cells["B1"].Value = "Employee Name";
        worksheet.Cells["C1"].Value = _localizer["lbl_Date"];
        worksheet.Cells["D1"].Value = "In Time";
        worksheet.Cells["E1"].Value = "Out Time";
        worksheet.Cells["F1"].Value = "D-Hours";
        worksheet.Cells["G1"].Value = "M-Hours";


        for (int i = 0; i < FaceAttendance.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = FaceAttendance[i].FaceAttendanceID;
          worksheet.Cells[i + 2, 2].Value = FaceAttendance[i].Employee?.FirstName + ' ' + FaceAttendance[i].Employee?.FatherName + ' ' + FaceAttendance[i].Employee?.FamilyName;
          worksheet.Cells[i + 2, 3].Value = FaceAttendance[i].MarkDate.ToString("dd/MMM/yyyy");
          worksheet.Cells[i + 2, 4].Value = FaceAttendance[i].InTime.ToString("hh:mm:ss");
          worksheet.Cells[i + 2, 5].Value = FaceAttendance[i].OutTime.ToString("hh:mm:ss");
          worksheet.Cells[i + 2, 6].Value = FaceAttendance[i].DHours;
          worksheet.Cells[i + 2, 7].Value = FaceAttendance[i].DMinutes;
        }

        worksheet.Cells["A1:l1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = $"FaceAttendance-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
    public async Task<IActionResult> Print()
    {
      var FaceAttendances = await _appDBContext.CR_FaceAttendances
        .Where(b => b.DeleteYNID != 1)
        .Include(d => d.Employee)
        .ToListAsync();
      return View("~/Views/HR/HR/FaceAttendanceForwarding/PrintFaceAttendanceForwarding.cshtml", FaceAttendances);
    }
  }
}

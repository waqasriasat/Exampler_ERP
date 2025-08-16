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
using System.Text.Json;

namespace Exampler_ERP.Controllers.HR.HR
{
  public class FaceAttendanceForwardingController : PositionController
  {
    private readonly AppDBContext _appDBContext;
    private readonly IStringLocalizer<FaceAttendanceForwardingController> _localizer;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    private readonly IHubContext<NotificationHub> _hubContext;


    public FaceAttendanceForwardingController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext, IStringLocalizer<FaceAttendanceForwardingController> localizer) 
    : base(appDBContext)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
      _hubContext = hubContext;
      _localizer = localizer;

    }
    public async Task<IActionResult> Index(int? Branch, int? MonthsTypeID, int? YearsTypeID)
    {
      if (!MonthsTypeID.HasValue && !YearsTypeID.HasValue)
      {
        var today = DateTime.Today;
        MonthsTypeID = today.Month;
        YearsTypeID = today.Year;
      }

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
    [RequestSizeLimit(100_000_000)]
    public async Task<JsonResult> UpdatePosted([FromBody] List<EmployeeData> Employees)
    {
      if (!ModelState.IsValid)
      {
        var errors = ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .ToList();

        return Json(new { success = false, message = "Binding failed", errors });
      }
        if (Employees == null)
        {
          return Json(new { success = false, message = "Employees is null — binding failed." });
        }
        else
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

            var groupedEmployees = Employees.GroupBy(emp => emp.employeeID);

            foreach (var group in groupedEmployees)
            {
              var employeeID = group.Key;
              var employee = group.First();

              int monthID = employee.markDate.Month;
              int year = employee.markDate.Year;

              int daysInMonth = DateTime.DaysInMonth(year, monthID);
              int weekends = CalculateWeekends(daysInMonth, monthID, year, employee.employeeID);

              int holidays = _appDBContext.HR_Holidays
                  .Where(h => h.HolidayDate.Month == monthID && h.HolidayDate.Year == year)
                  .Count();

              int workingDaysInWeek = _appDBContext.HR_GlobalSettings
              .Select(gs => (int?)gs.WorkingDayInWeek) // Cast to nullable int
              .FirstOrDefault() ?? 5;

              int totalWorkingDays = daysInMonth - weekends - holidays;

              int presentDays = _appDBContext.CR_FaceAttendances
                  .Where(a => a.EmployeeID == employee.employeeID && a.MarkDate.Month == monthID && a.MarkDate.Year == year)
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
                var AdditionalAllowance = _appDBContext.HR_AdditionalAllowances
                    .Where(a => a.EmployeeID == employeeID && a.MonthTypeID == monthID && a.Year == year)
                    .FirstOrDefault();

                if (AdditionalAllowance != null)
                {
                  var allowanceAmount = Math.Abs(absentDays) * perDaySalarys;
                  var allowanceDetail = new HR_AdditionalAllowanceDetail
                  {
                    AdditionalAllowanceID = AdditionalAllowance.AdditionalAllowanceID,
                    AdditionalAllowanceTypeID = 1,
                    AdditionalAllowanceAmount = allowanceAmount
                  };

                  _appDBContext.HR_AdditionalAllowanceDetails.Add(allowanceDetail);
                }
                else
                {
                  var newAllowance = new HR_AdditionalAllowance
                  {
                    EmployeeID = employeeID,
                    MonthTypeID = monthID,
                    Year = year,
                    FinalApprovalID = 1,
                    ProcessTypeApprovalID = 1,
                    PostedID = 0,
                    PayRollID = 0
                  };

                  _appDBContext.HR_AdditionalAllowances.Add(newAllowance);
                  _appDBContext.SaveChanges();

                  var allowanceAmount = Math.Abs(absentDays) * perDaySalarys;
                  var allowanceDetail = new HR_AdditionalAllowanceDetail
                  {
                    AdditionalAllowanceID = newAllowance.AdditionalAllowanceID,
                    AdditionalAllowanceTypeID = 3,
                    AdditionalAllowanceAmount = allowanceAmount
                  };

                  _appDBContext.HR_AdditionalAllowanceDetails.Add(allowanceDetail);
                }
              }
            }


            foreach (var employee in Employees)
            {
              int monthID = employee.markDate.Month;
              int year = employee.markDate.Year;
              if (employee.lateComingDeduction > 0)
              {
                var deduction = new HR_Deduction
                {
                  DeductionTypeID = 1,
                  EmployeeID = employee.employeeID,
                  Month = monthID,
                  Year = year,
                  Days = 1,
                  FromDate = employee.markDate,
                  ToDate = employee.markDate,
                  Amount = (float)employee.lateComingDeduction,
                  DeleteYNID = 0,
                  FinalApprovalID = 1
                };
                _appDBContext.HR_Deductions.Add(deduction);
              }

              if (employee.earlyGoingDeduction > 0)
              {
                var deduction = new HR_Deduction
                {
                  DeductionTypeID = 2,
                  EmployeeID = employee.employeeID,
                  Month = monthID,
                  Year = year,
                  Days = 1,
                  FromDate = employee.markDate,
                  ToDate = employee.markDate,
                  Amount = (float)employee.earlyGoingDeduction,
                  DeleteYNID = 0,
                  FinalApprovalID = 1
                };
                _appDBContext.HR_Deductions.Add(deduction);
              }

              if (employee.earlyComingAmount > 0)
              {
                var overtime = new HR_OverTime
                {
                  EmployeeID = employee.employeeID,
                  Amount = (float)employee.earlyComingAmount,
                  OverTimeTypeID = 1,
                  MonthTypeID = monthID,
                  Year = year,
                  Days = 1,
                  Hours = employee.earlyComingGraceTime,
                  DeleteYNID = 0,
                  FinalApprovalID = 1
                };
                _appDBContext.HR_OverTimes.Add(overtime);
              }

              if (employee.lateGoingAmount > 0)
              {
                var overtime = new HR_OverTime
                {
                  EmployeeID = employee.employeeID,
                  Amount = (float)employee.lateGoingAmount,
                  OverTimeTypeID = 1,
                  MonthTypeID = monthID,
                  Year = year,
                  Days = 1,
                  Hours = employee.lateGoingGraceTime,
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
   }

    // Model class for employee data
    public class EmployeeData
    {
      public int employeeID { get; set; }
      public DateTime markDate { get; set; }
      public decimal lateComingDeduction { get; set; }
      public decimal earlyGoingDeduction { get; set; }
      public int earlyComingGraceTime { get; set; }
      public decimal earlyComingAmount { get; set; }
      public int lateGoingGraceTime { get; set; }
      public decimal lateGoingAmount { get; set; }
    }




    public async Task<IActionResult> ExportToExcel(int? Branch, int? MonthsTypeID, int? YearsTypeID)
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

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


      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add(_localizer["lbl_FaceAttendance"]);
        worksheet.Cells["A1"].Value = _localizer["lbl_EmployeeName"];
        worksheet.Cells["B1"].Value = _localizer["lbl_Date"];
        worksheet.Cells["C1"].Value = _localizer["lbl_PresentTiming"];
        worksheet.Cells["D1"].Value = _localizer["lbl_AttendanceDuration"];
        worksheet.Cells["E1"].Value = _localizer["lbl_WorkingSlot"];
        worksheet.Cells["F1"].Value = _localizer["lbl_LateComingDeduction(Hours)"];
        worksheet.Cells["G1"].Value = _localizer["lbl_EarlyGoingDeduction(Hours)"];
        worksheet.Cells["H1"].Value = _localizer["lbl_EarlyComingAmount(Hours)"];
        worksheet.Cells["I1"].Value = _localizer["lbl_LateGoingAmount(Hours)"];

        for (int i = 0; i < attendanceRecords.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = attendanceRecords[i].Employee?.FirstName + ' ' + attendanceRecords[i].Employee?.FatherName + ' ' + attendanceRecords[i].Employee?.FamilyName;
          worksheet.Cells[i + 2, 2].Value = attendanceRecords[i].MarkDate.ToString("dd/MMM/yyyy");
          worksheet.Cells[i + 2, 3].Value = attendanceRecords[i].InTime + " - " + attendanceRecords[i].OutTime;
          worksheet.Cells[i + 2, 4].Value = attendanceRecords[i].DHours + " - " + attendanceRecords[i].DMinutes;
          worksheet.Cells[i + 2, 5].Value = attendanceRecords[i].FromDutyTime + " - " + attendanceRecords[i].ToDutyTime;
          worksheet.Cells[i + 2, 6].Value = attendanceRecords[i].LateComingDeduction + " (" + attendanceRecords[i].LateComingGraceTime + ")";
          worksheet.Cells[i + 2, 7].Value = attendanceRecords[i].EarlyGoingDeduction + " (" + attendanceRecords[i].EarlyGoingGraceTime + ")";
          worksheet.Cells[i + 2, 8].Value = attendanceRecords[i].EarlyComingAmount + " (" + attendanceRecords[i].EarlyComingGraceTime + ")";
          worksheet.Cells[i + 2, 9].Value = attendanceRecords[i].LateGoingAmount + " (" + attendanceRecords[i].LateGoingGraceTime + ")";
        }

        worksheet.Cells["A1:l1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = _localizer["lbl_FaceAttendance"]+$"-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
    public async Task<IActionResult> Print(int? Branch, int? MonthsTypeID, int? YearsTypeID)
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
      return View("~/Views/HR/HR/FaceAttendanceForwarding/PrintFaceAttendanceForwarding.cshtml", attendanceRecords);
    }
  }
}

using Exampler_ERP.Models;
using Exampler_ERP.Models.Temp;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Data;

namespace Exampler_ERP.Controllers.HR.HR
{
  public class FaceAttendanceForwardingController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;

    public FaceAttendanceForwardingController(AppDBContext appDBContext, IConfiguration configuration, Utils utils)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
    }
    public async Task<IActionResult> Index(int? Branch, int? MonthsTypeID, int? YearsTypeID)
    {
      var attendanceRecords = new List<FaceAttendanceForwardingViewModel>();
      var connectionString = _configuration.GetConnectionString("AppDb");
      using (SqlConnection connection = new SqlConnection(connectionString))
      {
        using (SqlCommand command = new SqlCommand("GetFaceAttendanceForwarding", connection))
        {
          command.CommandType = CommandType.StoredProcedure;
          command.Parameters.AddWithValue("@MonthID", MonthsTypeID ?? 10); // Default to October if null
          command.Parameters.AddWithValue("@YearID", YearsTypeID ?? 2024); // Default to 2024 if null
          command.Parameters.AddWithValue("@BranchID", Branch ?? 1); // Set your branch ID accordingly

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
                Console.WriteLine("InvalidCastException occurred: " + ex.Message);
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


    //public async Task<IActionResult> Index(int? MonthsTypeID, int? YearsTypeID, string? EmployeeName, int? EmployeeID)
    //{
    //  var query = _appDBContext.CR_FaceAttendances
    //       .Where(b => b.DeleteYNID != 1)
    //      .Include(d => d.Employee)
    //      .AsQueryable();

    //  if (MonthsTypeID.HasValue && MonthsTypeID != 0 && YearsTypeID.HasValue && YearsTypeID != 0)
    //  {
    //    var startDate = new DateTime(YearsTypeID.Value, MonthsTypeID.Value, 1);
    //    var endDate = startDate.AddMonths(1).AddDays(-1);  // Last day of the selected month

    //    query = query.Where(d => d.MarkDate >= startDate && d.MarkDate <= endDate);
    //  }

    //  if (EmployeeID.HasValue)
    //  {
    //    query = query.Where(d => d.EmployeeID == EmployeeID.Value);
    //  }

    //  if (!string.IsNullOrEmpty(EmployeeName))
    //  {
    //    query = query.Where(d =>
    //        (d.Employee.FirstName + " " + d.Employee.FatherName + " " + d.Employee.FamilyName)
    //        .Contains(EmployeeName));
    //  }


    //  var faceAttendance = await query.ToListAsync();

    //  ViewBag.MonthsTypeID = MonthsTypeID;
    //  ViewBag.YearsTypeID = YearsTypeID;
    //  ViewBag.EmployeeID = EmployeeID;
    //  ViewBag.EmployeeName = EmployeeName;

    //  ViewBag.MonthsTypeList = await _utils.GetMonthsTypes();

    //  return View("~/Views/HR/HR/FaceAttendanceForwarding/FaceAttendanceForwarding.cshtml", faceAttendance);
    //}
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
      ViewBag.EmployeesList = await _utils.GetEmployee();
      ViewBag.MonthsTypeList = await _utils.GetMonthsTypes();

      // Find the FaceAttendance by ID
      var FaceAttendance = await _appDBContext.CR_FaceAttendances
                                         .Include(d => d.Employee)
                                         .FirstOrDefaultAsync(d => d.FaceAttendanceID == id && d.DeleteYNID != 1);

      if (FaceAttendance == null)
      {
        return NotFound();
      }

      return PartialView("~/Views/HR/HR/FaceAttendanceForwarding/EditFaceAttendanceForwarding.cshtml", FaceAttendance);
    }
    [HttpPost]
    public async Task<IActionResult> Edit(CR_FaceAttendance FaceAttendance)
    {
      if (ModelState.IsValid)
      {
        try
        {
          var duration = FaceAttendance.OutTime - FaceAttendance.InTime;
          FaceAttendance.DHours = (int)duration.TotalHours;
          FaceAttendance.DMinutes = duration.Minutes;

          _appDBContext.CR_FaceAttendances.Update(FaceAttendance);
          await _appDBContext.SaveChangesAsync();
          return Json(new { success = true });
        }
        catch (Exception ex)
        {
          ModelState.AddModelError("", "Unable to save changes: " + ex.Message);
        }
      }

      ViewBag.EmployeesList = await _utils.GetEmployee();
      ViewBag.MonthsTypeList = await _utils.GetMonthsTypes();

      return PartialView("~/Views/HR/HR/FaceAttendanceForwarding/EditFaceAttendanceForwarding.cshtml", FaceAttendance);
    }
    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.EmployeesList = await _utils.GetEmployee();
      ViewBag.MonthsTypeList = await _utils.GetMonthsTypes();
      return PartialView("~/Views/HR/HR/FaceAttendanceForwarding/AddFaceAttendanceForwarding.cshtml", new CR_FaceAttendance());
    }
    [HttpPost]
    public async Task<IActionResult> Create(CR_FaceAttendance FaceAttendance)
    {
      if (ModelState.IsValid)
      {
        FaceAttendance.DeleteYNID = 0;


        //if(FaceAttendance.InTime != null && FaceAttendance.OutTime != null)
        {
          var duration = FaceAttendance.OutTime - FaceAttendance.InTime;
          FaceAttendance.DHours = (int)duration.TotalHours;
          FaceAttendance.DMinutes = duration.Minutes;
        }

        _appDBContext.CR_FaceAttendances.Add(FaceAttendance);
        await _appDBContext.SaveChangesAsync();
        return Json(new { success = true });
      }
      return PartialView("~/Views/HR/HR/FaceAttendanceForwarding/AddFaceAttendanceForwarding.cshtml", FaceAttendance);
    }
    public async Task<IActionResult> Delete(int id)
    {
      var FaceAttendance = await _appDBContext.CR_FaceAttendances.FindAsync(id);
      if (FaceAttendance == null)
      {
        return NotFound();
      }

      FaceAttendance.DeleteYNID = 1;

      _appDBContext.CR_FaceAttendances.Update(FaceAttendance);
      await _appDBContext.SaveChangesAsync();

      return Json(new { success = true });
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
        worksheet.Cells["C1"].Value = "Date";
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

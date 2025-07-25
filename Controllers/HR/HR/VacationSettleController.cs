using Exampler_ERP.Hubs;
using Exampler_ERP.Models;
using Exampler_ERP.Models.Temp;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;
using OfficeOpenXml;
using System.Globalization;
using Microsoft.Extensions.Localization;

namespace Exampler_ERP.Controllers.HR.HR
{
  public class VacationSettleController : PositionController
  {
    private readonly AppDBContext _appDBContext;
    private readonly IStringLocalizer<VacationSettleController> _localizer;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    private readonly IHubContext<NotificationHub> _hubContext;



    public VacationSettleController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext, IStringLocalizer<VacationSettleController> localizer) 
    : base(appDBContext)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
      _hubContext = hubContext;
      _localizer = localizer;


    }
    public async Task<IActionResult> Index(DateTime? FromDate, DateTime? ToDate, string? EmployeeName, int? EmployeeID, int? VacationTypeID)
    {
      var query = _appDBContext.HR_VacationSettles
        .Where(b => b.FinalApprovalID != 1)
        .Include(d => d.Vacation)
        .Include(d => d.Vacation.Settings_VacationType)
        .Include(d => d.Vacation.Employee)
        .OrderBy(emp => emp.VacationSettleID)
        .AsQueryable();

      if (FromDate.HasValue)
      {
        var fromDateTime = FromDate.Value.Date.AddSeconds(1);
        query = query.Where(d => d.Vacation.Date >= fromDateTime);
      }

      if (ToDate.HasValue)
      {
        var toDateTime = ToDate.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
        query = query.Where(d => d.Vacation.Date <= toDateTime);
      }

      if (EmployeeID.HasValue)
      {
        query = query.Where(d => d.Vacation.EmployeeID == EmployeeID.Value);
      }

      if (!string.IsNullOrEmpty(EmployeeName))
      {
        query = query.Where(d =>
            (d.Vacation.Employee.FirstName + " " + d.Vacation.Employee.FatherName + " " + d.Vacation.Employee.FamilyName)
            .Contains(EmployeeName));
      }

      if (VacationTypeID.HasValue && VacationTypeID != 0)
      {
        query = query.Where(d => d.Vacation.VacationTypeID == VacationTypeID.Value);
      }

      var vacationSettle = await query.ToListAsync();

      ViewBag.FromDate = FromDate;
      ViewBag.ToDate = ToDate;
      ViewBag.VacationTypeID = VacationTypeID;
      ViewBag.EmployeeID = EmployeeID;
      ViewBag.EmployeeName = EmployeeName;

      ViewBag.VacationTypeList = await _utils.GetVacationTypes();

      return View("~/Views/HR/HR/VacationSettle/VacationSettle.cshtml", vacationSettle);
    }
    // [HttpGet] Edit action method
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
      // Fetch the existing VacationSettle record by ID
      var vacationSettle = await _appDBContext.HR_VacationSettles
          .Where(vs => vs.VacationSettleID == id)
          .Include(d => d.Vacation)
          .Select(vs => new VacationSettleViewModel
          {
            VacationSettleID = vs.VacationSettleID,
            VacationID = vs.VacationID,
            EmployeeID = vs.Vacation.EmployeeID,
            StartDate = vs.Vacation.StartDate,
            EndDate = vs.Vacation.EndDate,
            TotalDays = vs.Vacation.TotalDays,
            SettleDays = vs.SettleDays,
            SettleAmount = vs.SettleAmount,
          })
          .FirstOrDefaultAsync();

      // If no record is found, return NotFound result
      if (vacationSettle == null)
      {
        return NotFound();
      }

      // Populate Employee and Vacation Date lists for the view
      ViewBag.EmployeesList = await _utils.GetEmployee();
      ViewBag.VacationDateList = await _utils.GetVacationDates();

      return PartialView("~/Views/HR/HR/VacationSettle/EditVacationSettle.cshtml", vacationSettle);
    }

    // [HttpPost] Edit action method
    [HttpPost]
    public async Task<IActionResult> Edit(VacationSettleViewModel model)
    {
      if (ModelState.IsValid)
      {
        var vacationSettle = await _appDBContext.HR_VacationSettles
            .FirstOrDefaultAsync(vs => vs.VacationID == model.VacationID);

        if (vacationSettle == null)
        {
          return NotFound();
        }

        vacationSettle.SettleDays = model.SettleDays;
        vacationSettle.SettleAmount = model.SettleAmount;

        _appDBContext.HR_VacationSettles.Update(vacationSettle);
        await _appDBContext.SaveChangesAsync();
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Vacation Settle updated successfully.");
        return Json(new { success = true });
      }
      await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "Error updating Vacation Settle. Please check the inputs.");
      return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.EmployeesList = await _utils.GetEmployee();
      ViewBag.VacationDateList = await _utils.GetVacationDates();

      return PartialView("~/Views/HR/HR/VacationSettle/AddVacationSettle.cshtml", new VacationSettleViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Create(VacationSettleViewModel model)
    {
      if (ModelState.IsValid)
      {

        var vacationSettle = new HR_VacationSettle
        {
          VacationID = model.VacationID,
          SettleDays = model.SettleDays,
          SettleAmount = model.SettleAmount,
        };

        _appDBContext.HR_VacationSettles.Add(vacationSettle);
        await _appDBContext.SaveChangesAsync();

        var vacationSettleId = vacationSettle.VacationSettleID;
        if (vacationSettleId > 0)
        {
          var processCount = await _appDBContext.CR_ProcessTypeApprovalSetups
                             .Where(pta => pta.ProcessTypeID > 0 && pta.ProcessTypeID == 12)
                             .CountAsync();

          if (processCount > 0)
          {
            var newProcessTypeApproval = new CR_ProcessTypeApproval
            {
              ProcessTypeID = 12,
              Notes = "Vacation Settle",
              Date = DateTime.Now,
              EmployeeID = model.EmployeeID,
              UserID = 1, // Using session value
              TransactionID = vacationSettleId
            };

            _appDBContext.CR_ProcessTypeApprovals.Add(newProcessTypeApproval);
            await _appDBContext.SaveChangesAsync();

            var nextApprovalSetup = await _appDBContext.CR_ProcessTypeApprovalSetups
                                     .Where(pta => pta.ProcessTypeID == 12 && pta.Rank == 1)
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
              await _hubContext.Clients.All.SendAsync("ReceiveProcessNotification");
              await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Vacation Settle Created successfully. Continue to the Approval Process Setup for Vacation Settle Activation.");
              return Json(new { success = true });
            }
            else
            {
              return Json(new { success = true });
            }
          }
          else
          {
            vacationSettle.FinalApprovalID = 1;
            vacationSettle.ProcessTypeApprovalID = 0;
            _appDBContext.HR_VacationSettles.Update(vacationSettle);
            await _appDBContext.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Vacation Settle Created successfully. No process setup found, Vacation Settle activated.");
            return Json(new { success = true });
          }
        }

        await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Vacation Settle Created successfully. Continue to the Approval Process Setup for Vacation Settle Activation.");
        return Json(new { success = true });
      }

      await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "Error creating Vacation Settle. Please check the inputs.");
      return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
    }
    [HttpGet]
    public async Task<IActionResult> GetVacationDatesByVacationID(int vacationID)
    {
      var vacationDates = await _utils.GetVacationDatesByVacationD(vacationID);
      return Json(vacationDates); // Return the data as JSON
    }
    [HttpGet]
    public async Task<IActionResult> GetVacationDatesByEmployeeID(int employeeID)
    {
      var vacationDates = await _utils.GetVacationDatesByEmployeeID(employeeID);
      return Json(vacationDates); // Return the data as JSON
    }
    [HttpGet]
    public async Task<IActionResult> GetVacationDatesByEmployeeIDWithoutSettled(int employeeID)
    {
      var vacationDates = await _utils.GetVacationDatesByEmployeeIDWithoutSettled(employeeID);
      return Json(vacationDates); // Return the data as JSON
    }
    [HttpGet]
    public async Task<IActionResult> GetVacationRecord(int vacationID, int employeeID)
    {
      var VacationBalanceresult = await _appDBContext.HR_Contracts
          .Where(emp => emp.ActiveYNID == 1 && emp.EmployeeID == employeeID)
          .Select(emp => new
          {
            VacationBalance = (EF.Functions.DateDiffDay(emp.StartDate, emp.EndDate) / 365.0) * emp.VacationDays
            - (_appDBContext.HR_Vacations
                .Where(vac => vac.EmployeeID == emp.Employee.EmployeeID
                              && vac.FinalApprovalID == 1
                              && _appDBContext.HR_VacationSettles.Any(vs => vs.VacationID == vac.VacationID))
                .Sum(vac => (int?)vac.TotalDays) ?? 0),
          })
          .FirstOrDefaultAsync();


      var vacation = await _appDBContext.HR_Vacations
          .Where(v => v.VacationID == vacationID)
          .Select(v => new
          {
            StartDate = v.StartDate.ToString("dd/MM/yyyy"), // Format to dd/MM/yyyy
            EndDate = v.EndDate.ToString("dd/MM/yyyy"),     // Format to dd/MM/yyyy
            TotalDays = v.TotalDays,
            SettleDays = (VacationBalanceresult.VacationBalance < v.TotalDays)
                  ? VacationBalanceresult.VacationBalance
                  : v.TotalDays


          })
          .FirstOrDefaultAsync();


      if (vacation == null)
      {
        return NotFound(); // Return 404 if the vacation record isn't found
      }
      var salarySum = await _appDBContext.HR_SalaryDetails
          .Where(sd => sd.Salary.EmployeeID == employeeID
                       && sd.Salary.FinalApprovalID == 1
                       && (sd.SalaryTypeID == 1 || sd.SalaryTypeID == 2))
          .SumAsync(sd => sd.SalaryAmount ?? 0);

      DateTime endDate = DateTime.ParseExact(vacation.EndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture); // Parse EndDate to DateTime
      int daysInMonth = DateTime.DaysInMonth(endDate.Year, endDate.Month);

      double dailySalary = salarySum / daysInMonth;

      double SettleAmount = dailySalary * vacation.SettleDays ?? 0.0;

      var result = new
      {
        vacation.StartDate,
        vacation.EndDate,
        vacation.TotalDays,
        vacation.SettleDays,
        SettleAmount = Math.Round(SettleAmount, 0) // Round to the nearest integer if required
      };

      return Json(result); // Return the vacation data as JSON
    }

    public async Task<IActionResult> Delete(int id)
    {
      var vacations = await _appDBContext.HR_Vacations.FindAsync(id);
      if (vacations == null)
      {
        return NotFound();
      }

      vacations.DeleteYNID = 1;

      _appDBContext.HR_Vacations.Update(vacations);
      await _appDBContext.SaveChangesAsync();
      await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Vacation Settle deleted successfully.");
      return Json(new { success = true });
    }

    public async Task<IActionResult> Print()
    {
      var employeeID = HttpContext.Session.GetInt32("EmployeeID");

      var vacations = await _appDBContext.HR_Vacations
                                   .Where(v => employeeID != null && v.EmployeeID == employeeID && v.DeleteYNID != 1)
                                   .OrderByDescending(v => v.VacationID)
                                   .Include(c => c.Employee)
                                    .Include(c => c.Settings_VacationType)
                                   .ToListAsync();

      return View("~/Views/HR/HR/VacationSettle/PrintVacationSettle.cshtml", vacations);
    }

    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var employeeID = HttpContext.Session.GetInt32("EmployeeID");

      var vacations = await _appDBContext.HR_Vacations
                                   .Where(v => employeeID != null && v.EmployeeID == employeeID && v.DeleteYNID != 1)
                                   .OrderByDescending(v => v.VacationID)
                                   .Include(c => c.Employee)
                                    .Include(c => c.Settings_VacationType)
                                   .ToListAsync();


      var vacationTypesList = await _utils.GetVacationTypes();
      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add(_localizer["lbl_VacationSettle"]);

        worksheet.Cells["A1"].Value = _localizer["lbl_VacationSettleID"];
        worksheet.Cells["B1"].Value = _localizer["lbl_EmployeeName"];
        worksheet.Cells["C1"].Value = _localizer["lbl_VacationTypeName"];
        worksheet.Cells["D1"].Value = _localizer["lbl_ApplyDate"];
        worksheet.Cells["E1"].Value = _localizer["lbl_StartDate"];
        worksheet.Cells["F1"].Value = _localizer["lbl_EndDate"];
        worksheet.Cells["G1"].Value = _localizer["lbl_TotalDays"];

        for (int i = 0; i < vacations.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = vacations[i].VacationID;
          worksheet.Cells[i + 2, 2].Value = vacations[i].Employee?.FirstName + ' ' + vacations[i].Employee?.FatherName + ' ' + vacations[i].Employee?.FamilyName;
          worksheet.Cells[i + 2, 3].Value = vacations[i].VacationTypeID == 0 || vacations[i]?.VacationTypeID == null
          ? ""
          : vacationTypesList.FirstOrDefault(g => g.Value == vacations[i].VacationTypeID.ToString())?.Text;
          worksheet.Cells[i + 2, 4].Value = vacations[i].Date.ToString("dd-MMM-yyyy");
          worksheet.Cells[i + 2, 5].Value = vacations[i].StartDate.ToString("dd-MMM-yyyy");
          worksheet.Cells[i + 2, 6].Value = vacations[i].EndDate.ToString("dd-MMM-yyyy");
          worksheet.Cells[i + 2, 7].Value = vacations[i].TotalDays;

        }

        worksheet.Cells["B1:G1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = _localizer["lbl_VacationSettle"]+$"-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
  }
}

using Exampler_ERP.Hubs;
using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;
using OfficeOpenXml;
using Microsoft.Extensions.Localization;

namespace Exampler_ERP.Controllers.HR.HR
{
  public class EndOfServiceController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IStringLocalizer<EndOfServiceController> _localizer;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    private readonly IHubContext<NotificationHub> _hubContext;



    public EndOfServiceController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext, IStringLocalizer<EndOfServiceController> localizer)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
      _hubContext = hubContext;
      _localizer = localizer;


    }
    public async Task<IActionResult> Index(DateTime? FromDate, DateTime? ToDate, string? EmployeeName, int? EmployeeID, int? EndOfServiceReasonTypeID)
    {
      var query = _appDBContext.HR_EndOfServices
        .Where(b => b.DeleteYNID != 1)
        .Include(d => d.Employee)
        .AsQueryable();

      if (FromDate.HasValue)
      {
        var fromDateTime = FromDate.Value.Date.AddSeconds(1);
        query = query.Where(d => d.DateOfCompletionOfWork >= fromDateTime);
      }

      if (ToDate.HasValue)
      {
        var toDateTime = ToDate.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
        query = query.Where(d => d.DateOfCompletionOfWork <= toDateTime);
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

      if (EndOfServiceReasonTypeID.HasValue && EndOfServiceReasonTypeID != 0)
      {
        query = query.Where(d => d.EndOfSerivceReasonTypeId == EndOfServiceReasonTypeID.Value);
      }

      var EndOfService = await query.ToListAsync();

      ViewBag.FromDate = FromDate;
      ViewBag.ToDate = ToDate;
      ViewBag.EndOfServiceReasonTypeID = EndOfServiceReasonTypeID;
      ViewBag.EmployeeID = EmployeeID;
      ViewBag.EmployeeName = EmployeeName;

      ViewBag.EndOfServiceReasonTypeList = await _utils.GetEndOfServiceReasonTypes();

      return View("~/Views/HR/HR/EndOfService/EndOfService.cshtml", EndOfService);
    }
    [HttpGet]
    public async Task<IActionResult> GetEmployeeDetails(int employeeID)
    {
      // Fetch employee details along with contract and contract type information
      var employeeDetails = await _appDBContext.HR_Contracts
          .Where(e => e.EmployeeID == employeeID)
          .Select(e => new
          {
            HireDate = e.Employee.HireDate.HasValue
                  ? e.Employee.HireDate.Value.ToString("MM/dd/yyyy")
                  : null,
            ContractTypeName = e.Settings_ContractType != null
                  ? e.Settings_ContractType.ContractTypeName
                  : "Unknown"
          })
          .FirstOrDefaultAsync();

      // Fetch basic salary detail for the employee
      var basicSalaryDetail = await _appDBContext.HR_SalaryDetails
          .Include(sd => sd.Salary)
          .Include(sd => sd.SalaryType)
          .Where(sd => sd.Salary.EmployeeID == employeeID && sd.SalaryType.SalaryTypeName == "Basic Salary")
          .Select(sd => new
          {
            SalaryAmount = sd.SalaryAmount ?? 0
          })
          .FirstOrDefaultAsync();

      if (employeeDetails == null)
      {
        return NotFound(); // Return 404 if no employee details found
      }

      // Combine employeeDetails and basicSalaryDetail in a single response
      var result = new
      {
        HireDate = employeeDetails.HireDate,
        ContractTypeName = employeeDetails.ContractTypeName,
        BasicSalary = basicSalaryDetail?.SalaryAmount ?? 0 // Handle null case for basic salary
      };

      return Json(result); // Return the combined result as JSON
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.EmployeesList = await _utils.GetEmployee();
      ViewBag.EndOfServiceReasonTypesList = await _utils.GetEndOfServiceReasonTypes();
      return PartialView("~/Views/HR/HR/EndOfService/AddEndOfService.cshtml", new HR_EndOfService());
    }
    [HttpPost]
    public async Task<IActionResult> Create(HR_EndOfService endofservice)
    {
      if (ModelState.IsValid)
      {
        endofservice.DeleteYNID = 0;
        _appDBContext.HR_EndOfServices.Add(endofservice);
        await _appDBContext.SaveChangesAsync(); // Save EndOfService first

        var endOfServiceID = endofservice.EndOfServiceID;
        if (endOfServiceID > 0)
        {
          // Check if there is any approval process setup for End of Service (ProcessTypeID = 5)
          var processCount = await _appDBContext.CR_ProcessTypeApprovalSetups
                                                .CountAsync(pta => pta.ProcessTypeID == 5);

          if (processCount > 0)
          {
            // Create new process type approval entry
            var newProcessTypeApproval = new CR_ProcessTypeApproval
            {
              ProcessTypeID = 5,
              Notes = "End Of Service",
              Date = DateTime.Now,
              EmployeeID = endofservice.EmployeeID,
              UserID = HttpContext.Session.GetInt32("UserID") ?? 0,
              TransactionID = endOfServiceID
            };

            _appDBContext.CR_ProcessTypeApprovals.Add(newProcessTypeApproval);
            await _appDBContext.SaveChangesAsync(); // Save the process type approval

            // Get the next approval setup with rank 1 for the process type
            var nextApprovalSetup = await _appDBContext.CR_ProcessTypeApprovalSetups
                                                       .Where(pta => pta.ProcessTypeID == 5 && pta.Rank == 1)
                                                       .FirstOrDefaultAsync();

            if (nextApprovalSetup != null)
            {
              // Create new process type approval detail
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
              await _appDBContext.SaveChangesAsync(); // Save the process type approval detail
              await _hubContext.Clients.All.SendAsync("ReceiveProcessNotification");
            }
            else
            {
              return Json(new { success = false, message = "Next approval setup not found." });
            }
          }
          else
          {
            // If no process setup, deactivate the contract and update employee status
            var contractToUpdate = _appDBContext.HR_Contracts
                                                .FirstOrDefault(c => c.EmployeeID == endofservice.EmployeeID);

            if (contractToUpdate != null)
            {
              contractToUpdate.ActiveYNID = 2;
              _appDBContext.SaveChanges();
            }

            var employee = await _appDBContext.HR_Employees
                                              .FirstOrDefaultAsync(e => e.EmployeeID == endofservice.EmployeeID);

            if (employee != null)
            {
              employee.EmployeeStatusTypeID = 5; // End of Service status
              employee.ActiveYNID = 2; // Deactivate employee
            }
            await _appDBContext.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "EndOfService Created successfully. No process setup found, Employee deactivated.");
            return Json(new { success = true, message = "No process setup found, Employee deactivated." });
          }
        }

        await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "EndOfService Created successfully. Continue to the Approval Process Setup for Employee deactivated.");
        return Json(new { success = true });
      }
      await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "Error creating EndOfService. Please check the inputs.");
      return PartialView("~/Views/HR/HR/EndOfService/AddEndOfService.cshtml", endofservice);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
      var endOfService = await _appDBContext.HR_EndOfServices
          .Include(e => e.Employee)
          .FirstOrDefaultAsync(e => e.EndOfServiceID == id);

      if (endOfService == null)
      {
        return NotFound();
      }

      ViewBag.EmployeesList = await _utils.GetEmployee();
      ViewBag.EndOfServiceReasonTypesList = await _utils.GetEndOfServiceReasonTypes();
      return PartialView("~/Views/HR/HR/EndOfService/EditEndOfService.cshtml", endOfService);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(HR_EndOfService endofservice)
    {
      if (ModelState.IsValid)
      {
        _appDBContext.Update(endofservice);
        await _appDBContext.SaveChangesAsync();
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "EndOfService Updated successfully.");
        return Json(new { success = true });
      }

      ViewBag.EmployeesList = await _utils.GetEmployee();
      ViewBag.EndOfServiceReasonTypesList = await _utils.GetEndOfServiceReasonTypes();
      await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "Error Updating EndOfService. Please check the inputs.");
      return PartialView("~/Views/HR/HR/EndOfService/EditEndOfService.cshtml", endofservice);
    }
    public async Task<IActionResult> Delete(int id)
    {
      var endofservice = await _appDBContext.HR_EndOfServices.FindAsync(id);
      if (endofservice == null)
      {
        return NotFound();
      }

      endofservice.DeleteYNID = 1;

      _appDBContext.HR_EndOfServices.Update(endofservice);
      await _appDBContext.SaveChangesAsync();
      await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Employee Deleted successfully.");
      return Json(new { success = true });
    }
    public async Task<IActionResult> Print()
    {
      var EndOfService = await _appDBContext.HR_EndOfServices
        .Where(b => b.DeleteYNID != 1)
        .Include(d => d.Employee)
        .Include(c => c.Settings_EndOfSerivceReasonType)
        .ToListAsync();
      return View("~/Views/HR/HR/EndOfService/PrintEndOfService.cshtml", EndOfService);
    }

    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var EndOfService = await _appDBContext.HR_EndOfServices
          .Where(c => c.DeleteYNID != 1)
          .Include(c => c.Employee)
          .Include(c => c.Settings_EndOfSerivceReasonType)
          .ToListAsync();

      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add(_localizer["lbl_EndOfService"]);

        worksheet.Cells["A1"].Value = _localizer["lbl_EndOfServiceID"];
        worksheet.Cells["B1"].Value = _localizer["lbl_EmployeeName"];
        worksheet.Cells["C1"].Value = _localizer["lbl_ReasonType"];
        worksheet.Cells["D1"].Value = _localizer["lbl_TotalSalary"];
        worksheet.Cells["E1"].Value = _localizer["lbl_TotalDayOfAbsent"];
        worksheet.Cells["F1"].Value = _localizer["lbl_DateOfCommencement"];
        worksheet.Cells["G1"].Value = _localizer["lbl_DateOfCompletionOfWork"];
        worksheet.Cells["H1"].Value = _localizer["lbl_NumberofYears"];
        worksheet.Cells["I1"].Value = _localizer["lbl_NumberofMonths"];
        worksheet.Cells["J1"].Value = _localizer["lbl_NumberofDays"];
        worksheet.Cells["K1"].Value = _localizer["lbl_TotalDays"];
        worksheet.Cells["L1"].Value = _localizer["lbl_EndofServiceBenefit"];
        worksheet.Cells["M1"].Value = _localizer["lbl_BalanceOfTheAnnualLeave"];
        worksheet.Cells["N1"].Value = _localizer["lbl_AmountDuesForTheLeave"];
        worksheet.Cells["O1"].Value = _localizer["lbl_TotalEndOfServiceDues"];

        for (int i = 0; i < EndOfService.Count; i++)
        {
          int employeeID = EndOfService[i].EmployeeID;

          // Query for fetching the basic salary
          var basicSalaryDetail = await _appDBContext.HR_SalaryDetails
              .Include(sd => sd.Salary)
              .Include(sd => sd.SalaryType)
              .Where(sd => sd.Salary.EmployeeID == employeeID && sd.SalaryType.SalaryTypeName == "Basic Salary")
              .Select(sd => new
              {
                SalaryAmount = sd.SalaryAmount ?? 0 // Default to 0 if SalaryAmount is null
              })
              .FirstOrDefaultAsync();

          // Set Excel worksheet values
          worksheet.Cells[i + 2, 1].Value = EndOfService[i].EndOfServiceID;
          worksheet.Cells[i + 2, 2].Value = EndOfService[i].Employee?.FirstName + ' ' + EndOfService[i].Employee?.FatherName + ' ' + EndOfService[i].Employee?.FamilyName;
          worksheet.Cells[i + 2, 3].Value = EndOfService[i].Settings_EndOfSerivceReasonType?.EndOfServiceReasonTypeName;

          // Set the basic salary amount from the query result
          worksheet.Cells[i + 2, 4].Value = basicSalaryDetail?.SalaryAmount ?? 0;

          worksheet.Cells[i + 2, 5].Value = ""; // Empty value for this cell
          worksheet.Cells[i + 2, 6].Value = EndOfService[i].Employee?.HireDate?.ToString("dd-MMM-yyyy");
          worksheet.Cells[i + 2, 7].Value = EndOfService[i].DateOfCompletionOfWork.ToString("dd-MMM-yyyy");
          worksheet.Cells[i + 2, 8].Value = EndOfService[i].NumberofYears;
          worksheet.Cells[i + 2, 9].Value = EndOfService[i].NumberofMonths;
          worksheet.Cells[i + 2, 10].Value = EndOfService[i].NumberofDays;
          worksheet.Cells[i + 2, 11].Value = EndOfService[i].TotalDays;
          worksheet.Cells[i + 2, 12].Value = EndOfService[i].EndofServiceBenefit;
          worksheet.Cells[i + 2, 13].Value = EndOfService[i].BalanceOfTheAnnualLeave;
          worksheet.Cells[i + 2, 14].Value = EndOfService[i].AmountDueForTheLeave;
          worksheet.Cells[i + 2, 15].Value = EndOfService[i].TotalEndOfServiceDues;
        }


        worksheet.Cells["A1:J1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = _localizer["lbl_EndOfService"]+$"-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
  }
}

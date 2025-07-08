using Exampler_ERP.Models;
using Exampler_ERP.Models.Temp;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Exampler_ERP.Controllers.MasterInfo
{
  public class ApprovalsRequestController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
private readonly IHubContext<NotificationHub> _hubContext;

    public ApprovalsRequestController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
_hubContext = hubContext;
 
    }
    
    public async Task<IActionResult> Index(DateTime? FromDate, DateTime? ToDate, string? EmployeeName, int? EmployeeID, int? ProcessTypeID)
    {
      var query = _appDBContext.CR_ProcessTypeApprovalDetails
          .Include(pta => pta.CR_ProcessTypeApproval)
              .ThenInclude(pta => pta.Employee)
          .Include(pta => pta.CR_ProcessTypeApproval)
              .ThenInclude(pta => pta.ProcessType)
          .Include(pta => pta.ProcessTypeApprovalDetailDoc)
          .Where(pta => pta.AppID == 0);

      if (FromDate.HasValue)
      {
        var fromDateTime = FromDate.Value.Date.AddSeconds(1);
        query = query.Where(pta => pta.CR_ProcessTypeApproval.Date >= fromDateTime);
      }

      if (ToDate.HasValue)
      {
        var toDateTime = ToDate.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
        query = query.Where(pta => pta.CR_ProcessTypeApproval.Date <= toDateTime);
      }

      // Filter by EmployeeName
      if (EmployeeID.HasValue)
      {
        query = query.Where(pta => pta.CR_ProcessTypeApproval.EmployeeID <= EmployeeID.Value);
      }

      // Filter by ProcessTypeID
      if (ProcessTypeID.HasValue && ProcessTypeID != 0)
      {
        query = query.Where(pta => pta.CR_ProcessTypeApproval.ProcessTypeID == ProcessTypeID.Value);
      }

      ViewBag.FromDate = FromDate;
      ViewBag.ToDate = ToDate;
      ViewBag.ProcessTypeID = ProcessTypeID;
      ViewBag.EmployeeID = EmployeeID;
      ViewBag.EmployeeName = EmployeeName;

      var result = await query
          .OrderByDescending(pta => pta.ProcessTypeApprovalDetailID)
          .ToListAsync();

      ViewBag.ProcessTypeList = await _utils.GetProcessTypes();
      return View("~/Views/MasterInfo/ApprovalsRequest/ApprovalsRequestSearching.cshtml", result);
    }
    public async Task<IActionResult> SelectedIndex(int? id = null)
    {
      var query = _appDBContext.CR_ProcessTypeApprovalDetails
          .Include(pta => pta.CR_ProcessTypeApproval)
              .ThenInclude(pta => pta.Employee)
          .Include(pta => pta.CR_ProcessTypeApproval)
              .ThenInclude(pta => pta.ProcessType)
          .Include(pta => pta.ProcessTypeApprovalDetailDoc)
          .Where(pta => pta.AppID == 0);

      if (id.HasValue)
      {
        query = query.Where(pta => pta.CR_ProcessTypeApproval.ProcessTypeID == id.Value);
      }

      var result = await query
          .OrderByDescending(pta => pta.ProcessTypeApprovalDetailID)
          .ToListAsync();

      ViewBag.ProcessTypeList = await _utils.GetProcessTypes();
      return View("~/Views/MasterInfo/ApprovalsRequest/ApprovalsRequest.cshtml", result);
    }
    [HttpGet]
    public async Task<IActionResult> DownloadDocument(int id)
    {
      var document = await _appDBContext.CR_ProcessTypeApprovalDetailDocs
          .FirstOrDefaultAsync(d => d.ProcessTypeApprovalDetailDocID == id);

      if (document == null || document.Doc == null)
      {
        return NotFound();
      }

      return File(document.Doc, "application/octet-stream", document.DocName);
    }
    [HttpGet]
    public async Task<IActionResult> ViewDocument(int id)
    {
      // Retrieve the document based on ProcessTypeApprovalDetailDocID
      var document = await _appDBContext.CR_ProcessTypeApprovalDetailDocs
          .FirstOrDefaultAsync(d => d.ProcessTypeApprovalDetailDocID == id);

      if (document == null || document.Doc == null)
      {
        // If document is not found, return a 404 error.
        return NotFound();
      }

      // Determine MIME type based on file extension
      string mimeType = document.DocExt.ToLower() switch
      {
        ".pdf" => "application/pdf",
        ".jpg" => "image/jpeg",
        ".jpeg" => "image/jpeg",
        ".png" => "image/png",
        _ => "application/octet-stream"
      };

      // Return the document for inline viewing
      return File(document.Doc, mimeType, document.DocName);
    }
    public async Task<IActionResult> Approvals(int id)
    {
    

      var result = await _appDBContext.CR_ProcessTypeApprovalDetails
    .Include(pta => pta.CR_ProcessTypeApproval) 
    .ThenInclude(pta => pta.Employee)
    .Include(pta => pta.CR_ProcessTypeApproval) 
    .ThenInclude(pta => pta.ProcessType)
    .Include(pta => pta.ProcessTypeApprovalDetailDoc) 
    .Where(pta => pta.AppID == 1 && pta.ProcessTypeApprovalID == id)
    .OrderBy(pta => pta.Rank) 
    .ToListAsync();

      ViewBag.EmployeesList = await _utils.GetEmployee();
      ViewBag.RolesList = await _utils.GetRoles();


      if (result == null)
      {
        return NotFound($"No approval record found for ProcessTypeApprovalID = {id}");
      }

      return PartialView("~/Views/MasterInfo/ApprovalsRequest/ApprovalsProcessTypeApproval.cshtml", result);
    }
    public async Task<IActionResult> Actions(int id)
    {
      var result = await _appDBContext.CR_ProcessTypeApprovalDetails
               .Where(pta => pta.ProcessTypeApprovalDetailID == id)
               .Include(pta => pta.CR_ProcessTypeApproval)
               .FirstOrDefaultAsync();

      var ProcessTypeID = result?.CR_ProcessTypeApproval?.ProcessTypeID;
      var TransactionID = result?.CR_ProcessTypeApproval?.TransactionID;
      var RankID = result?.Rank;
      var ProcessTypeApprovalDetailID = result?.ProcessTypeApprovalDetailID;

      ViewBag.ProcessTypeID = ProcessTypeID;
      ViewBag.TransactionID = TransactionID;
      ViewBag.RankID = RankID;
      ViewBag.ProcessTypeApprovalDetailID = ProcessTypeApprovalDetailID;


      if (result == null)
      {
        return NotFound($"No approval record found for ProcessTypeApprovalID = {id}");
      }

      return PartialView("~/Views/MasterInfo/ApprovalsRequest/ActionsProcessTypeApproval.cshtml", result);
    }
    [HttpPost]
    public async Task<IActionResult> Approved(CR_ProcessTypeApprovalDetail processTypeApprovalDetail, IFormFile FileUpload, int ProcessTypeID, int TransactionID, string Password)
    {
      if (ModelState.IsValid)
      {
        if (Password == null)
        {
          return Json(new { success = false, message = "Password is required. Please enter your password to continue." });
        }
        else
        {
          var userId = HttpContext.Session.GetInt32("UserID");

          if (userId != null)
          {
            var currentUser = await _appDBContext.CR_Users
                .Where(pta => pta.UserID == userId)
                .FirstOrDefaultAsync();

            if(CR_CipherKey.Encrypt(Password) == currentUser.Password)
            {
              processTypeApprovalDetail.AppID = 1;
              processTypeApprovalDetail.AppUserID = HttpContext.Session.GetInt32("UserID") ?? default(int);
              processTypeApprovalDetail.Date = DateTime.Now;

              if (FileUpload != null && FileUpload.Length > 0)
              {
                using (var memoryStream = new MemoryStream())
                {
                  await FileUpload.CopyToAsync(memoryStream);

                  var processTypeApprovalDetailDoc = new CR_ProcessTypeApprovalDetailDoc
                  {
                    ProcessTypeApprovalDetailID = processTypeApprovalDetail.ProcessTypeApprovalDetailID,
                    Doc = memoryStream.ToArray(),
                    DocName = FileUpload.FileName,
                    DocExt = Path.GetExtension(FileUpload.FileName)
                  };

                  processTypeApprovalDetail.ProcessTypeApprovalDetailDoc.Add(processTypeApprovalDetailDoc);
                }
              }

              _appDBContext.Update(processTypeApprovalDetail);
              await _appDBContext.SaveChangesAsync();

              var processTypeId = ProcessTypeID;
              var transactionID = TransactionID;
              var currentRank = processTypeApprovalDetail.Rank;

              var nextApprovalSetup = await _appDBContext.CR_ProcessTypeApprovalSetups
                  .Where(pta => pta.ProcessTypeID == processTypeId && pta.Rank == currentRank + 1)
                  .FirstOrDefaultAsync();

              if (nextApprovalSetup != null)
              {
                var newApprovalSetup = new CR_ProcessTypeApprovalDetail
                {
                  ProcessTypeApprovalID = processTypeApprovalDetail.ProcessTypeApprovalID,
                  Date = processTypeApprovalDetail.Date,
                  RoleID = nextApprovalSetup.RoleTypeID,
                  AppID = 0,
                  AppUserID = 0,
                  Notes = null,
                  Rank = nextApprovalSetup.Rank
                };

                _appDBContext.CR_ProcessTypeApprovalDetails.Add(newApprovalSetup);
                await _appDBContext.SaveChangesAsync();
              }
              else
              {
                if (ProcessTypeID == 1)
                {
                  var user = await _appDBContext.CR_Users
                  .Where(u => u.UserID == transactionID)
                  .FirstOrDefaultAsync();

                  if (user != null)
                  {
                    user.ActiveYNID = 1;
                    user.FinalApprovalID = 1;
                    user.ProcessTypeApprovalID = processTypeApprovalDetail.ProcessTypeApprovalID;

                    _appDBContext.Update(user);
                    await _appDBContext.SaveChangesAsync();
                  }
                }
                if (ProcessTypeID == 2)
                {
                  var employee = await _appDBContext.HR_Employees
                  .Where(u => u.EmployeeID == transactionID)
                  .FirstOrDefaultAsync();

                  if (employee != null)
                  {
                    employee.ActiveYNID = 1;
                    employee.FinalApprovalID = 1;
                    employee.ProcessTypeApprovalID = processTypeApprovalDetail.ProcessTypeApprovalID;

                    _appDBContext.Update(employee);
                    await _appDBContext.SaveChangesAsync();
                  }
                }
                if (ProcessTypeID == 3)
                {
                  var contract = await _appDBContext.HR_Contracts
                  .Where(u => u.ContractID == transactionID)
                  .FirstOrDefaultAsync();

                  if (contract != null)
                  {
                    contract.ActiveYNID = 1;
                    contract.FinalApprovalID = 1;
                    contract.ProcessTypeApprovalID = processTypeApprovalDetail.ProcessTypeApprovalID;

                    _appDBContext.Update(contract);
                    await _appDBContext.SaveChangesAsync();
                  }
                }
                if (ProcessTypeID == 4)
                {
                  var contractRenewal = await _appDBContext.HR_ContractRenewals
                                                           .Where(u => u.ContractRenewalID == transactionID)
                                                           .FirstOrDefaultAsync();

                  if (contractRenewal != null)
                  {
                    contractRenewal.FinalApprovalID = 1;
                    contractRenewal.ProcessTypeApprovalID = processTypeApprovalDetail.ProcessTypeApprovalID;
                    await _appDBContext.SaveChangesAsync();
                  }

                  var contract = await _appDBContext.HR_Contracts
                                                    .Where(u => u.ContractID == contractRenewal.ContractID)
                                                    .FirstOrDefaultAsync();

                  if (contract != null)
                  {
                    contract.EndDate = contractRenewal.NEndDate;
                    await _appDBContext.SaveChangesAsync();
                  }
                }
                if (ProcessTypeID == 5)
                {
                  var endOfService = await _appDBContext.HR_EndOfServices
                                                           .Where(u => u.EndOfServiceID == transactionID)
                                                           .FirstOrDefaultAsync();

                  if (endOfService != null)
                  {
                    endOfService.FinalApprovalID = 1;
                    endOfService.ProcessTypeApprovalID = processTypeApprovalDetail.ProcessTypeApprovalID;
                    await _appDBContext.SaveChangesAsync();
                  }

                  var contractToUpdate = _appDBContext.HR_Contracts
                                                     .FirstOrDefault(c => c.EmployeeID == endOfService.EmployeeID);

                  if (contractToUpdate != null)
                  {
                    contractToUpdate.ActiveYNID = 2;
                    _appDBContext.SaveChanges();
                  }

                  var employee = await _appDBContext.HR_Employees
                                                    .FirstOrDefaultAsync(e => e.EmployeeID == endOfService.EmployeeID);

                  if (employee != null)
                  {
                    employee.EmployeeStatusTypeID = 5; // End of Service status
                    employee.ActiveYNID = 2; // Deactivate employee
                  }
                  await _appDBContext.SaveChangesAsync();
                }
                if (ProcessTypeID == 6)
                {
                  var salary = await _appDBContext.HR_Salarys
                                                           .Where(u => u.SalaryID == transactionID)
                                                           .FirstOrDefaultAsync();


                  if (salary != null)
                  {
                    salary.FinalApprovalID = 1;
                    salary.ProcessTypeApprovalID = processTypeApprovalDetail.ProcessTypeApprovalID;

                    _appDBContext.Update(salary);
                    await _appDBContext.SaveChangesAsync();
                  }
                }
                if (ProcessTypeID == 7)
                {
                  var overTime = await _appDBContext.HR_OverTimes
                                                           .Where(u => u.OverTimeID == transactionID)
                                                           .FirstOrDefaultAsync();


                  if (overTime != null)
                  {
                    overTime.FinalApprovalID = 1;
                    overTime.ProcessTypeApprovalID = processTypeApprovalDetail.ProcessTypeApprovalID;

                    _appDBContext.Update(overTime);
                    await _appDBContext.SaveChangesAsync();
                  }
                }
                if (ProcessTypeID == 8)
                {
                  var addionalAllowance = await _appDBContext.HR_AddionalAllowances
                                                           .Where(u => u.AddionalAllowanceID == transactionID)
                                                           .FirstOrDefaultAsync();


                  if (addionalAllowance != null)
                  {
                    addionalAllowance.FinalApprovalID = 1;
                    addionalAllowance.ProcessTypeApprovalID = processTypeApprovalDetail.ProcessTypeApprovalID;

                    _appDBContext.Update(addionalAllowance);
                    await _appDBContext.SaveChangesAsync();
                  }
                }
                if (ProcessTypeID == 9)
                {
                  var deduction = await _appDBContext.HR_Deductions
                                                          .Where(u => u.DeductionID == transactionID)
                                                          .FirstOrDefaultAsync();


                  if (deduction != null)
                  {
                    deduction.FinalApprovalID = 1;
                    deduction.ProcessTypeApprovalID = processTypeApprovalDetail.ProcessTypeApprovalID;

                    _appDBContext.Update(deduction);
                    await _appDBContext.SaveChangesAsync();
                  }
                }
                if (ProcessTypeID == 10)
                {
                  var fixedDeduction = await _appDBContext.HR_FixedDeductions
                                                           .Where(u => u.FixedDeductionID == transactionID)
                                                           .FirstOrDefaultAsync();


                  if (fixedDeduction != null)
                  {
                    fixedDeduction.FinalApprovalID = 1;
                    fixedDeduction.ProcessTypeApprovalID = processTypeApprovalDetail.ProcessTypeApprovalID;

                    _appDBContext.Update(fixedDeduction);
                    await _appDBContext.SaveChangesAsync();
                  }
                }
                if (ProcessTypeID == 11)
                {
                  var vacation = await _appDBContext.HR_Vacations
                                                           .Where(u => u.VacationID == transactionID)
                                                           .FirstOrDefaultAsync();


                  if (vacation != null)
                  {
                    vacation.FinalApprovalID = 1;
                    vacation.ProcessTypeApprovalID = processTypeApprovalDetail.ProcessTypeApprovalID;

                    _appDBContext.Update(vacation);
                    await _appDBContext.SaveChangesAsync();
                  }
                }
                if (ProcessTypeID == 12)
                {
                  var vacationSettle = await _appDBContext.HR_VacationSettles
                                                           .Where(u => u.VacationSettleID == transactionID)
                                                           .FirstOrDefaultAsync();


                  if (vacationSettle != null)
                  {
                    vacationSettle.FinalApprovalID = 1;
                    vacationSettle.ProcessTypeApprovalID = processTypeApprovalDetail.ProcessTypeApprovalID;

                    _appDBContext.Update(vacationSettle);
                    await _appDBContext.SaveChangesAsync();
                  }
                }
                if (ProcessTypeID == 13)
                {
                  var monthlyPayrollPosted = await _appDBContext.HR_MonthlyPayrollPosteds
                                                           .Where(u => u.PayrollPostedID == transactionID)
                                                           .FirstOrDefaultAsync();


                  if (monthlyPayrollPosted != null)
                  {
                    var monthlyPayroll = new HR_MonthlyPayroll
                    {
                      BranchTypeID = monthlyPayrollPosted.BranchTypeID,
                      MonthTypeID = monthlyPayrollPosted.MonthTypeID,
                      Year = monthlyPayrollPosted.Year
                    };

                    _appDBContext.HR_MonthlyPayrolls.Add(monthlyPayroll);
                    await _appDBContext.SaveChangesAsync();
                    var additionalAllowances = await _appDBContext.HR_AddionalAllowances
                    .Where(a => a.MonthTypeID == monthlyPayrollPosted.MonthTypeID && a.Year == monthlyPayrollPosted.Year)
                    .ToListAsync();

                    foreach (var allowance in additionalAllowances)
                    {
                      allowance.PayRollID = monthlyPayroll.PayrollID;
                      allowance.PostedID = monthlyPayrollPosted.PayrollPostedID;
                    }

                    var overTimes = await _appDBContext.HR_OverTimes
                        .Where(o => o.MonthTypeID == monthlyPayrollPosted.MonthTypeID && o.Year == monthlyPayrollPosted.Year)
                        .ToListAsync();

                    foreach (var overtime in overTimes)
                    {
                      overtime.PayRollID = monthlyPayroll.PayrollID;
                      overtime.PostedID = monthlyPayrollPosted.PayrollPostedID;
                    }

                    var hrDeductions = await _appDBContext.HR_Deductions
                        .Where(d => d.Month == monthlyPayrollPosted.MonthTypeID && d.Year == monthlyPayrollPosted.Year)
                        .ToListAsync();

                    foreach (var deduction in hrDeductions)
                    {
                      deduction.PayRollID = monthlyPayroll.PayrollID;
                      deduction.PostedID = monthlyPayrollPosted.PayrollPostedID;
                    }

                    await _appDBContext.SaveChangesAsync();





                    var salaryDetails = await _appDBContext.HR_SalaryDetails
                    .Where(sd => sd.Salary.Employee.BranchTypeID == monthlyPayrollPosted.BranchTypeID)
                    .Include(sd => sd.Salary)
                    .Include(sd => sd.Salary.Employee)
                    .ToListAsync();

                    foreach (var salaryGroup in salaryDetails.GroupBy(sd => sd.Salary.EmployeeID))
                    {
                      var firstSalaryDetail = salaryGroup.FirstOrDefault();
                      if (firstSalaryDetail?.Salary != null)
                      {
                        var monthlyPayroll_Salary = new HR_MonthlyPayroll_Salary
                        {
                          EmployeeID = firstSalaryDetail.Salary.EmployeeID,
                          MonthTypeID = monthlyPayrollPosted.MonthTypeID,
                          Year = monthlyPayrollPosted.Year,
                          PostedID = monthlyPayrollPosted.PayrollPostedID,
                          PayRollID = monthlyPayroll.PayrollID // Assuming this is available
                        };

                        _appDBContext.HR_MonthlyPayroll_Salarys.Add(monthlyPayroll_Salary);
                        await _appDBContext.SaveChangesAsync();

                        foreach (var salaryDetail in salaryGroup)
                        {
                          var monthlyPayroll_SalaryDetail = new HR_MonthlyPayroll_SalaryDetail
                          {
                            PayrollSalaryID = monthlyPayroll_Salary.PayrollSalaryID, // Use the same PayrollSalaryID
                            SalaryTypeID = salaryDetail.SalaryTypeID,
                            SalaryAmount = salaryDetail.SalaryAmount
                          };

                          _appDBContext.HR_MonthlyPayroll_SalaryDetails.Add(monthlyPayroll_SalaryDetail);
                        }

                        await _appDBContext.SaveChangesAsync();
                      }
                    }


                    var FixedDeductionDetails = await _appDBContext.HR_FixedDeductionDetails
                   .Where(sd => sd.FixedDeduction.Employee.BranchTypeID == monthlyPayrollPosted.BranchTypeID)
                   .Include(sd => sd.FixedDeduction)
                   .Include(sd => sd.FixedDeduction.Employee)
                   .ToListAsync();

                    foreach (var FixedDeductionGroup in FixedDeductionDetails.GroupBy(sd => sd.FixedDeduction.EmployeeID))
                    {
                      var firstFixedDeductionDetail = FixedDeductionGroup.FirstOrDefault();
                      if (firstFixedDeductionDetail?.FixedDeduction != null)
                      {
                        var monthlyPayroll_FixedDeduction = new HR_MonthlyPayroll_FixedDeduction
                        {
                          EmployeeID = firstFixedDeductionDetail.FixedDeduction.EmployeeID,
                          MonthTypeID = monthlyPayrollPosted.MonthTypeID,
                          Year = monthlyPayrollPosted.Year,
                          PostedID = monthlyPayrollPosted.PayrollPostedID,
                          PayRollID = monthlyPayroll.PayrollID // Assuming this is available
                        };

                        _appDBContext.HR_MonthlyPayroll_FixedDeductions.Add(monthlyPayroll_FixedDeduction);
                        await _appDBContext.SaveChangesAsync();

                        foreach (var FixedDeductionDetail in FixedDeductionGroup)
                        {
                          var monthlyPayroll_FixedDeductionDetail = new HR_MonthlyPayroll_FixedDeductionDetail
                          {
                            PayrollFixedDeductionID = monthlyPayroll_FixedDeduction.PayrollFixedDeductionID, // Use the same PayrollFixedDeductionID
                            FixedDeductionTypeID = FixedDeductionDetail.FixedDeductionTypeID,
                            FixedDeductionAmount = FixedDeductionDetail.FixedDeductionAmount
                          };

                          _appDBContext.HR_MonthlyPayroll_FixedDeductionDetails.Add(monthlyPayroll_FixedDeductionDetail);
                        }

                        await _appDBContext.SaveChangesAsync();
                      }
                    }

                    monthlyPayrollPosted.FinalApprovalID = 1;
                    monthlyPayrollPosted.ProcessTypeApprovalID = processTypeApprovalDetail.ProcessTypeApprovalID;

                    _appDBContext.Update(monthlyPayrollPosted);
                    await _appDBContext.SaveChangesAsync();
                  }
                }
                if (ProcessTypeID == 14)
                {
                  //var EmployeeRequest = await _appDBContext.HR_EmployeeRequests
                  //                                         .Where(u => u.EmployeeRequestID == transactionID)
                  //                                         .FirstOrDefaultAsync();


                  //if (EmployeeRequest != null)
                  //{
                  //  EmployeeRequest.FinalApprovalID = 1;
                  //  EmployeeRequest.ProcessTypeApprovalID = processTypeApprovalDetail.ProcessTypeApprovalID;

                  //  _appDBContext.Update(EmployeeRequest);
                  //  await _appDBContext.SaveChangesAsync();
                  //}
                }
                if (ProcessTypeID == 15)
                {
                }
                if (ProcessTypeID == 16)
                {
                  var vouchers = await _appDBContext.FI_Vouchers
                                                           .Where(u => u.VoucherID == transactionID)
                                                           .FirstOrDefaultAsync();


                  if (vouchers != null)
                  {
                    vouchers.FinalApprovalID = 1;
                    vouchers.ProcessTypeApprovalID = processTypeApprovalDetail.ProcessTypeApprovalID;

                    _appDBContext.Update(vouchers);
                    await _appDBContext.SaveChangesAsync();
                  }
                }
                if (ProcessTypeID == 17)
                {
                  var vouchers = await _appDBContext.FI_Vouchers
                                                           .Where(u => u.VoucherID == transactionID)
                                                           .FirstOrDefaultAsync();


                  if (vouchers != null)
                  {
                    vouchers.FinalApprovalID = 1;
                    vouchers.ProcessTypeApprovalID = processTypeApprovalDetail.ProcessTypeApprovalID;

                    _appDBContext.Update(vouchers);
                    await _appDBContext.SaveChangesAsync();
                  }
                }
                if (ProcessTypeID == 18)
                {
                  var vouchers = await _appDBContext.FI_Vouchers
                                                           .Where(u => u.VoucherID == transactionID)
                                                           .FirstOrDefaultAsync();


                  if (vouchers != null)
                  {
                    vouchers.FinalApprovalID = 1;
                    vouchers.ProcessTypeApprovalID = processTypeApprovalDetail.ProcessTypeApprovalID;

                    _appDBContext.Update(vouchers);
                    await _appDBContext.SaveChangesAsync();
                  }
                }
                if (ProcessTypeID == 19)
                {
                  var vouchers = await _appDBContext.FI_Vouchers
                                                           .Where(u => u.VoucherID == transactionID)
                                                           .FirstOrDefaultAsync();


                  if (vouchers != null)
                  {
                    vouchers.FinalApprovalID = 1;
                    vouchers.ProcessTypeApprovalID = processTypeApprovalDetail.ProcessTypeApprovalID;

                    _appDBContext.Update(vouchers);
                    await _appDBContext.SaveChangesAsync();
                  }
                }
                if (ProcessTypeID == 20)
                {
                  var requisitio = await _appDBContext.ST_MaterialRequisitions
                                                           .Where(u => u.RequisitionID == transactionID)
                                                           .FirstOrDefaultAsync();


                  if (requisitio != null)
                  {
                    requisitio.FinalApprovalID = 1;
                    requisitio.ProcessTypeApprovalID = processTypeApprovalDetail.ProcessTypeApprovalID;

                    _appDBContext.Update(requisitio);
                    await _appDBContext.SaveChangesAsync();
                  }
                }
                if (ProcessTypeID == 21)
                {
                  var purchaseRequest = await _appDBContext.PR_PurchaseRequests
                                                           .Where(u => u.PurchaseRequestID == transactionID)
                                                           .FirstOrDefaultAsync();


                  if (purchaseRequest != null)
                  {
                    purchaseRequest.FinalApprovalID = 1;
                    purchaseRequest.ProcessTypeApprovalID = processTypeApprovalDetail.ProcessTypeApprovalID;

                    _appDBContext.Update(purchaseRequest);
                    await _appDBContext.SaveChangesAsync();
                  }
                }
                if (ProcessTypeID == 22)
                {
                  var purchaseRequest = await _appDBContext.PR_PurchaseRequests
                                                           .Where(u => u.PurchaseRequestID == transactionID)
                                                           .FirstOrDefaultAsync();


                  if (purchaseRequest != null)
                  {
                    purchaseRequest.FinalApprovalID = 1;
                    purchaseRequest.ProcessTypeApprovalID = processTypeApprovalDetail.ProcessTypeApprovalID;

                    _appDBContext.Update(purchaseRequest);
                    await _appDBContext.SaveChangesAsync();
                  }
                }
                if (ProcessTypeID == 23)
                {
                  var purchaseOrder = await _appDBContext.PR_PurchaseOrders
                                                           .Where(u => u.PurchaseRequestID == transactionID)
                                                           .FirstOrDefaultAsync();


                  if (purchaseOrder != null)
                  {
                    purchaseOrder.FinalApprovalID = 1;
                    purchaseOrder.ProcessTypeApprovalID = processTypeApprovalDetail.ProcessTypeApprovalID;

                    _appDBContext.Update(purchaseOrder);
                    await _appDBContext.SaveChangesAsync();
                  }
                }
              }
              await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Successfully Approved.");
              return Json(new { success = true });
            }
            else
            {
              return Json(new { success = false, message = "The password you entered is incorrect. Please try again." });
            }
          }
          
        }

       
      }
      return Json(new { success = false, message = "Error updating Approval. Please check the inputs." });
    }
    [HttpPost]
    public async Task<IActionResult> Reject(CR_ProcessTypeApprovalDetail processTypeApprovalDetail, IFormFile FileUpload, int ProcessTypeID, int TransactionID, string Password, string Motes)
    {
      if (ModelState.IsValid)
      {
        processTypeApprovalDetail.AppID = 1;
        processTypeApprovalDetail.AppUserID = HttpContext.Session.GetInt32("UserID") ?? default(int);
        processTypeApprovalDetail.Date = DateTime.Now;

        if (FileUpload != null && FileUpload.Length > 0)
        {
          using (var memoryStream = new MemoryStream())
          {
            await FileUpload.CopyToAsync(memoryStream);

            var processTypeApprovalDetailDoc = new CR_ProcessTypeApprovalDetailDoc
            {
              ProcessTypeApprovalDetailID = processTypeApprovalDetail.ProcessTypeApprovalDetailID,
              Doc = memoryStream.ToArray(),
              DocName = FileUpload.FileName,
              DocExt = Path.GetExtension(FileUpload.FileName)
            };

            processTypeApprovalDetail.ProcessTypeApprovalDetailDoc.Add(processTypeApprovalDetailDoc);
          }
        }

        _appDBContext.Update(processTypeApprovalDetail);
        await _appDBContext.SaveChangesAsync();

        var processTypeId = ProcessTypeID;
        var transactionID = TransactionID;
          if (ProcessTypeID == 1)
          {
            var user = await _appDBContext.CR_Users
            .Where(u => u.UserID == transactionID)
            .FirstOrDefaultAsync();

            if (user != null)
            {
              user.ActiveYNID = 2;
              user.FinalApprovalID = 2;
              user.ProcessTypeApprovalID = processTypeApprovalDetail.ProcessTypeApprovalID;

              _appDBContext.Update(user);
              await _appDBContext.SaveChangesAsync();
            }
        }
        if (ProcessTypeID == 2)
        {
          var employee = await _appDBContext.HR_Employees
          .Where(u => u.EmployeeID == transactionID)
          .FirstOrDefaultAsync();

          if (employee != null)
          {
            employee.ActiveYNID = 2;
            employee.FinalApprovalID = 2;
            employee.ProcessTypeApprovalID = processTypeApprovalDetail.ProcessTypeApprovalID;

            _appDBContext.Update(employee);
            await _appDBContext.SaveChangesAsync();
          }
        }
        if (ProcessTypeID == 3)
        {
          var contract = await _appDBContext.HR_Contracts
          .Where(u => u.ContractID == transactionID)
          .FirstOrDefaultAsync();

          if (contract != null)
          {
            contract.ActiveYNID = 2;
            contract.FinalApprovalID = 2;
            contract.ProcessTypeApprovalID = processTypeApprovalDetail.ProcessTypeApprovalID;

            _appDBContext.Update(contract);
            await _appDBContext.SaveChangesAsync();
          }
          if (ProcessTypeID == 4)
          {
            var contractRenewal = await _appDBContext.HR_ContractRenewals
                                                     .Where(u => u.ContractRenewalID == transactionID)
                                                     .FirstOrDefaultAsync();

            if (contractRenewal != null)
            {
              contractRenewal.FinalApprovalID = 2;
              contractRenewal.ProcessTypeApprovalID = processTypeApprovalDetail.ProcessTypeApprovalID;

              _appDBContext.Update(contractRenewal);
              await _appDBContext.SaveChangesAsync();
            }
          }
          if (ProcessTypeID == 5)
          {
            var endOfService = await _appDBContext.HR_EndOfServices
                                                     .Where(u => u.EndOfServiceID == transactionID)
                                                     .FirstOrDefaultAsync();

            if (endOfService != null)
            {
              endOfService.FinalApprovalID = 2;
              endOfService.ProcessTypeApprovalID = processTypeApprovalDetail.ProcessTypeApprovalID;

              _appDBContext.Update(endOfService);
              await _appDBContext.SaveChangesAsync();
            }
          }
          if (ProcessTypeID == 6)
          {
            var salary = await _appDBContext.HR_Salarys
                                                     .Where(u => u.SalaryID == transactionID)
                                                     .FirstOrDefaultAsync();

            if (salary != null)
            {
              salary.FinalApprovalID = 2;
              salary.ProcessTypeApprovalID = processTypeApprovalDetail.ProcessTypeApprovalID;

              _appDBContext.Update(salary);
              await _appDBContext.SaveChangesAsync();
            }
          }
          if (ProcessTypeID == 7)
          {
            var overTime = await _appDBContext.HR_OverTimes
                                                     .Where(u => u.OverTimeID == transactionID)
                                                     .FirstOrDefaultAsync();


            if (overTime != null)
            {
              overTime.FinalApprovalID = 2;
              overTime.ProcessTypeApprovalID = processTypeApprovalDetail.ProcessTypeApprovalID;

              _appDBContext.Update(overTime);
              await _appDBContext.SaveChangesAsync();
            }
          }
          if (ProcessTypeID == 8)
          {
            var addionalAllowance = await _appDBContext.HR_AddionalAllowances
                                                     .Where(u => u.AddionalAllowanceID == transactionID)
                                                     .FirstOrDefaultAsync();


            if (addionalAllowance != null)
            {
              addionalAllowance.FinalApprovalID = 2;
              addionalAllowance.ProcessTypeApprovalID = processTypeApprovalDetail.ProcessTypeApprovalID;

              _appDBContext.Update(addionalAllowance);
              await _appDBContext.SaveChangesAsync();
            }
          }
          if (ProcessTypeID == 9)
          {
            var deduction = await _appDBContext.HR_Deductions
                                                    .Where(u => u.DeductionID == transactionID)
                                                    .FirstOrDefaultAsync();


            if (deduction != null)
            {
              deduction.FinalApprovalID = 2;
              deduction.ProcessTypeApprovalID = processTypeApprovalDetail.ProcessTypeApprovalID;

              _appDBContext.Update(deduction);
              await _appDBContext.SaveChangesAsync();
            }
          }
          if (ProcessTypeID == 10)
          {
            var fixedDeduction = await _appDBContext.HR_FixedDeductions
                                                     .Where(u => u.FixedDeductionID == transactionID)
                                                     .FirstOrDefaultAsync();


            if (fixedDeduction != null)
            {
              fixedDeduction.FinalApprovalID = 2;
              fixedDeduction.ProcessTypeApprovalID = processTypeApprovalDetail.ProcessTypeApprovalID;

              _appDBContext.Update(fixedDeduction);
              await _appDBContext.SaveChangesAsync();
            }

          }
          if (ProcessTypeID == 11)
          {
            var vacation = await _appDBContext.HR_Vacations
                                                     .Where(u => u.VacationID == transactionID)
                                                     .FirstOrDefaultAsync();


            if (vacation != null)
            {
              vacation.FinalApprovalID = 2;
              vacation.ProcessTypeApprovalID = processTypeApprovalDetail.ProcessTypeApprovalID;

              _appDBContext.Update(vacation);
              await _appDBContext.SaveChangesAsync();
            }
          }
          if (ProcessTypeID == 12)
          {
            var vacationSettle = await _appDBContext.HR_VacationSettles
                                                    .Where(u => u.VacationSettleID == transactionID)
                                                    .FirstOrDefaultAsync();


            if (vacationSettle != null)
            {
              vacationSettle.FinalApprovalID = 2;
              vacationSettle.ProcessTypeApprovalID = processTypeApprovalDetail.ProcessTypeApprovalID;

              _appDBContext.Update(vacationSettle);
              await _appDBContext.SaveChangesAsync();
            }
          }
          if (ProcessTypeID == 13)
          {
            var monthlyPayrollPosted = await _appDBContext.HR_MonthlyPayrollPosteds
                                                     .Where(u => u.PayrollPostedID == transactionID)
                                                     .FirstOrDefaultAsync();


            if (monthlyPayrollPosted != null)
            {
          
              monthlyPayrollPosted.FinalApprovalID = 2;
              monthlyPayrollPosted.ProcessTypeApprovalID = processTypeApprovalDetail.ProcessTypeApprovalID;

              _appDBContext.Update(monthlyPayrollPosted);
              await _appDBContext.SaveChangesAsync();
            }
          }
          if (ProcessTypeID == 14)
          {
            //var EmployeeRequest = await _appDBContext.HR_EmployeeRequests
            //                                         .Where(u => u.EmployeeRequestID == transactionID)
            //                                         .FirstOrDefaultAsync();


            //if (EmployeeRequest != null)
            //{
            //  EmployeeRequest.FinalApprovalID = 2;
            //  EmployeeRequest.ProcessTypeApprovalID = processTypeApprovalDetail.ProcessTypeApprovalID;

            //  _appDBContext.Update(EmployeeRequest);
            //  await _appDBContext.SaveChangesAsync();
            //}
          }
          if (ProcessTypeID == 15)
          {
          }
          if (ProcessTypeID == 16)
          {
            var vouchers = await _appDBContext.FI_Vouchers
                                                           .Where(u => u.VoucherID == transactionID)
                                                           .FirstOrDefaultAsync();


            if (vouchers != null)
            {
              vouchers.FinalApprovalID = 2;
              vouchers.ProcessTypeApprovalID = processTypeApprovalDetail.ProcessTypeApprovalID;

              _appDBContext.Update(vouchers);
              await _appDBContext.SaveChangesAsync();
            }
          }
          if (ProcessTypeID == 17)
          {
            var vouchers = await _appDBContext.FI_Vouchers
                                                           .Where(u => u.VoucherID == transactionID)
                                                           .FirstOrDefaultAsync();


            if (vouchers != null)
            {
              vouchers.FinalApprovalID = 2;
              vouchers.ProcessTypeApprovalID = processTypeApprovalDetail.ProcessTypeApprovalID;

              _appDBContext.Update(vouchers);
              await _appDBContext.SaveChangesAsync();
            }
          }
          if (ProcessTypeID == 18)
          {
            var vouchers = await _appDBContext.FI_Vouchers
                                                           .Where(u => u.VoucherID == transactionID)
                                                           .FirstOrDefaultAsync();


            if (vouchers != null)
            {
              vouchers.FinalApprovalID = 2;
              vouchers.ProcessTypeApprovalID = processTypeApprovalDetail.ProcessTypeApprovalID;

              _appDBContext.Update(vouchers);
              await _appDBContext.SaveChangesAsync();
            }
          }
          if (ProcessTypeID == 19)
          {
            var vouchers = await _appDBContext.FI_Vouchers
                                                           .Where(u => u.VoucherID == transactionID)
                                                           .FirstOrDefaultAsync();


            if (vouchers != null)
            {
              vouchers.FinalApprovalID = 2;
              vouchers.ProcessTypeApprovalID = processTypeApprovalDetail.ProcessTypeApprovalID;

              _appDBContext.Update(vouchers);
              await _appDBContext.SaveChangesAsync();
            }
          }
          if (ProcessTypeID == 20)
          {
            var requisitio = await _appDBContext.ST_MaterialRequisitions
                                                     .Where(u => u.RequisitionID == transactionID)
                                                     .FirstOrDefaultAsync();


            if (requisitio != null)
            {
              requisitio.FinalApprovalID = 2;
              requisitio.ProcessTypeApprovalID = processTypeApprovalDetail.ProcessTypeApprovalID;

              _appDBContext.Update(requisitio);
              await _appDBContext.SaveChangesAsync();
            }
          }
          if (ProcessTypeID == 21)
          {
            var purchaseRequest = await _appDBContext.PR_PurchaseRequests
                                                     .Where(u => u.PurchaseRequestID == transactionID)
                                                     .FirstOrDefaultAsync();


            if (purchaseRequest != null)
            {
              purchaseRequest.FinalApprovalID = 2;
              purchaseRequest.ProcessTypeApprovalID = processTypeApprovalDetail.ProcessTypeApprovalID;

              _appDBContext.Update(purchaseRequest);
              await _appDBContext.SaveChangesAsync();
            }
          }
          if (ProcessTypeID == 22)
          {
            var purchaseRequest = await _appDBContext.PR_PurchaseRequests
                                                     .Where(u => u.PurchaseRequestID == transactionID)
                                                     .FirstOrDefaultAsync();


            if (purchaseRequest != null)
            {
              purchaseRequest.FinalApprovalID = 2;
              purchaseRequest.ProcessTypeApprovalID = processTypeApprovalDetail.ProcessTypeApprovalID;

              _appDBContext.Update(purchaseRequest);
              await _appDBContext.SaveChangesAsync();
            }
          }
          if (ProcessTypeID == 23)
          {
            var purchaseOrder = await _appDBContext.PR_PurchaseOrders
                                                     .Where(u => u.PurchaseRequestID == transactionID)
                                                     .FirstOrDefaultAsync();


            if (purchaseOrder != null)
            {
              purchaseOrder.FinalApprovalID = 2;
              purchaseOrder.ProcessTypeApprovalID = processTypeApprovalDetail.ProcessTypeApprovalID;

              _appDBContext.Update(purchaseOrder);
              await _appDBContext.SaveChangesAsync();
            }
          }
        }
        return Json(new { success = true });
      }

      return PartialView("~/Views/MasterInfo/ApprovalsRequest/ActionsProcessTypeApproval.cshtml", processTypeApprovalDetail);
    }
    public async Task<IActionResult> Details(int id)
    {
      // Fetch the ProcessTypeApproval record
      var fatchDetail = await _appDBContext.CR_ProcessTypeApprovals
          .Where(pta => pta.ProcessTypeApprovalID == id)
          .FirstOrDefaultAsync();

      if (fatchDetail == null)
      {
        return NotFound();
      }

      var processTypeID = fatchDetail.ProcessTypeID;
      var transactionID = fatchDetail.TransactionID;

      // Set ProcessTypeID and TransactionID in ViewBag
      ViewBag.ProcessTypeID = processTypeID;
      ViewBag.TransactionID = transactionID;

      object result = null;

      // Handle for ProcessTypeID == 1 (User)
      if (processTypeID == 1)
      {
        ViewBag.RoleList = await _utils.GetRoles();
        ViewBag.EmployeeList = await _utils.GetEmployee();

        var user = await _appDBContext.CR_Users.FindAsync(transactionID);
        if (user == null)
        {
          return NotFound();
        }

        // Decrypt user data if needed
        user.UserName = CR_CipherKey.Decrypt(user.UserName);

        return PartialView("~/Views/MasterInfo/ApprovalsRequest/DetailsProcessTypeApproval.cshtml", user);
      }

      // Handle for ProcessTypeID == 2 (Employee)
      if (processTypeID == 2)
      {
        ViewBag.GenderList = await _utils.GetGender();
        ViewBag.MaritalStatusList = await _utils.GetMaritalStatus();
        ViewBag.ReligionList = await _utils.GetReligion();
        ViewBag.CountriesList = _utils.GetCountries();
        ViewBag.BranchsList = await _utils.GetBranchs();
        ViewBag.DepartmentsList = await _utils.GetDepartments();
        ViewBag.DesignationsList = await _utils.GetDesignations();

        var employee = await _appDBContext.HR_Employees.FindAsync(transactionID);
        if (employee == null)
        {
          return NotFound();
        }

        return PartialView("~/Views/MasterInfo/ApprovalsRequest/DetailsProcessTypeApproval.cshtml", employee);
      }
      if (processTypeID == 3)
      {
        var contract = await _appDBContext.HR_Contracts.FindAsync(transactionID);
        if (contract == null)
        {
          return NotFound();
        }

        ViewBag.EmployeesList = await _utils.GetEmployee();
        ViewBag.SalaryTypesList = await _utils.GetSalaryOptions();
        ViewBag.ContractTypesList = await _utils.GetContractTypes();

        return PartialView("~/Views/MasterInfo/ApprovalsRequest/DetailsProcessTypeApproval.cshtml", contract);
      }
      if (processTypeID == 4)
      {
        var contractRenewal = await _appDBContext.HR_ContractRenewals.FindAsync(transactionID);
        if (contractRenewal == null)
        {
          return NotFound();
        }

        ViewBag.EmployeesList = await _utils.GetEmployee();

        return PartialView("~/Views/MasterInfo/ApprovalsRequest/DetailsProcessTypeApproval.cshtml", contractRenewal);
      }
      if (processTypeID == 5)
      {
        var endOfService = await _appDBContext.HR_EndOfServices
           .Include(e => e.Employee)
           .FirstOrDefaultAsync(e => e.EndOfServiceID == transactionID);

        if (endOfService == null)
        {
          return NotFound();
        }

        ViewBag.EmployeesList = await _utils.GetEmployee();
        ViewBag.EndOfServiceReasonTypesList = await _utils.GetEndOfServiceReasonTypes();

        return PartialView("~/Views/MasterInfo/ApprovalsRequest/DetailsProcessTypeApproval.cshtml", endOfService);

      }
      if (processTypeID == 6)
      {
   

        var salaryDetails = await _appDBContext.HR_SalaryDetails
       .Where(pt => pt.SalaryID == transactionID)
       .Include(pt => pt.Salary.Employee)
       .ToListAsync();

        ViewBag.SalaryTypeList = await _appDBContext.Settings_SalaryTypes
            .Select(r => new { Value = r.SalaryTypeID, Text = r.SalaryTypeName })
            .ToListAsync();

        return PartialView("~/Views/MasterInfo/ApprovalsRequest/DetailsProcessTypeApproval.cshtml", salaryDetails);
      }
      if (processTypeID == 7)
      {
        var OverTime = await _appDBContext.HR_OverTimes
                                         .Include(d => d.Employee)
                                         .Include(d => d.OverTimeType)
                                         .FirstOrDefaultAsync(d => d.OverTimeID == transactionID && d.DeleteYNID != 1 );
        if (OverTime == null)
        {
          return NotFound(); // Handle not found case
        }

        ViewBag.EmployeesList = await _utils.GetEmployee();
        ViewBag.OverTimeTypesList = await _utils.GetOverTimeTypes();
        ViewBag.OvertimeRatesList = await _utils.GetOverTimeRates();
        ViewBag.MonthsList = await _utils.GetMonthsTypes();

        return PartialView("~/Views/MasterInfo/ApprovalsRequest/DetailsProcessTypeApproval.cshtml", OverTime);
      }
      if (processTypeID == 8)
      {
        var allowance = await _appDBContext.HR_AddionalAllowances
         .Include(a => a.AddionalAllowanceDetails) // Include the details for editing
         .FirstOrDefaultAsync(a => a.AddionalAllowanceID == transactionID);

        if (allowance == null)
        {
          return NotFound(); // Handle not found case
        }

        // Prepare necessary ViewBag data for the edit view
        ViewBag.AddionalAllowanceTypeList = await _appDBContext.Settings_AddionalAllowanceTypes
            .Select(r => new { Value = r.AddionalAllowanceTypeID, Text = r.AddionalAllowanceTypeName })
            .ToListAsync();

        ViewBag.EmployeesList = await _utils.GetEmployee();
        ViewBag.MonthsList = await _utils.GetMonthsTypes();

        return PartialView("~/Views/MasterInfo/ApprovalsRequest/DetailsProcessTypeApproval.cshtml", allowance);

      }
      if (processTypeID == 9)
      {
        var deduction = await _appDBContext.HR_Deductions
                                         .Include(d => d.Employee)
                                         .Include(d => d.DeductionType)
                                         .FirstOrDefaultAsync(d => d.DeductionID == transactionID && d.DeleteYNID != 1);

        if (deduction == null)
        {
          return NotFound();
        }


        ViewBag.EmployeesList = await _utils.GetEmployee();
        ViewBag.DeductionTypesList = await _utils.GetDeductionTypes();

        return PartialView("~/Views/MasterInfo/ApprovalsRequest/DetailsProcessTypeApproval.cshtml", deduction);
      }
      if (processTypeID == 10)
      {
        var FixedDeductionDetails = await _appDBContext.HR_FixedDeductionDetails
      .Where(pt => pt.FixedDeductionID == transactionID)
      .Include(pt => pt.FixedDeduction.Employee)
      .ToListAsync();

        ViewBag.FixedDeductionTypeList = await _appDBContext.Settings_FixedDeductionTypes
            .Select(r => new { Value = r.FixedDeductionTypeID, Text = r.FixedDeductionTypeName })
            .ToListAsync();

        return PartialView("~/Views/MasterInfo/ApprovalsRequest/DetailsProcessTypeApproval.cshtml", FixedDeductionDetails);
      }
      if (processTypeID == 11)
      {
        ViewBag.VacationTypeList = await _utils.GetVacationTypes();
        var vacations = await _appDBContext.HR_Vacations
        .Where(v => v.VacationID == transactionID)
                                     .Include(c => c.Employee)
                                      .Include(c => c.Settings_VacationType)
                                     .FirstOrDefaultAsync();

        return PartialView("~/Views/MasterInfo/ApprovalsRequest/DetailsProcessTypeApproval.cshtml", vacations);
      }
      if (processTypeID == 12)
      {
        var vacationSettle = await _appDBContext.HR_VacationSettles
          .Where(vs => vs.VacationSettleID == transactionID)
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

        return PartialView("~/Views/MasterInfo/ApprovalsRequest/DetailsProcessTypeApproval.cshtml", vacationSettle);
      }
      if (processTypeID == 13)
      {
        var FatchExistingPosting = await _appDBContext.HR_MonthlyPayrollPosteds
       .Where(e => e.PayrollPostedID == transactionID)
       .FirstOrDefaultAsync();

        var employeeList = await _appDBContext.HR_Employees
          .Where(e => e.ActiveYNID == 1 && e.FinalApprovalID == 1 && e.BranchTypeID == FatchExistingPosting.BranchTypeID)
          .ToListAsync();

        var salarySheets = new List<MonthlySalarySheetViewModel>();
        var tasks = employeeList.Select(async employee =>
        {
          var salaryData = await GetMonthlySalarySheetAsync(employee.EmployeeID.ToString(), FatchExistingPosting.MonthTypeID ?? 0, FatchExistingPosting.Year ?? 0);
          return salaryData;
        });
        var salaryDataResults = await Task.WhenAll(tasks);
        salarySheets.AddRange(salaryDataResults);


        return PartialView("~/Views/MasterInfo/ApprovalsRequest/DetailsProcessTypeApproval.cshtml", salarySheets);
      }
      if (processTypeID == 14)
      {
        //ViewBag.EmployeeRequestTypeList = await _utils.GetEmployeeRequestTypes();
        //var EmployeeRequests = await _appDBContext.HR_EmployeeRequests
        //.Where(v => v.EmployeeRequestID == transactionID)
        //                             .Include(c => c.Employee)
        //                              .Include(c => c.Settings_EmployeeRequestType)
        //                             .FirstOrDefaultAsync();
        //return PartialView("~/Views/MasterInfo/ApprovalsRequest/DetailsProcessTypeApproval.cshtml", EmployeeRequests);
      }
      if (processTypeID == 15)
      {
      }
      if (processTypeID == 16)
      {
        var Vouchers = await _appDBContext.FI_Vouchers
                  .Include(v => v.VoucherDetails)
                  .FirstOrDefaultAsync(v => v.VoucherID == transactionID);

        if (Vouchers == null)
        {
          return NotFound();
        }

        Vouchers.VoucherDetails.Add(new FI_VoucherDetail() { VoucherID = Vouchers.VoucherID });

        var model = new JournalVoucherIndexViewModel
        {
          Vouchers = Vouchers
        };

        ViewBag.VoucherTypeList = await _utils.GetVoucherType_Journal();
        ViewBag.TransactionTypeList = await _utils.GetTransactionType();
        ViewBag.HeadofAccount_FiveList = await _utils.GetHeadofAccount_Five();

        return PartialView("~/Views/MasterInfo/ApprovalsRequest/DetailsProcessTypeApproval.cshtml", model);
      }
      if (processTypeID == 17)
      {
        var Vouchers = await _appDBContext.FI_Vouchers
                  .Include(v => v.VoucherDetails)
                  .FirstOrDefaultAsync(v => v.VoucherID == transactionID);

        if (Vouchers == null)
        {
          return NotFound();
        }

        Vouchers.VoucherDetails.Add(new FI_VoucherDetail() { VoucherID = Vouchers.VoucherID });

        var model = new TransferVoucherIndexViewModel
        {
          Vouchers = Vouchers
        };

        ViewBag.VoucherTypeList = await _utils.GetVoucherType_Transfer();
        ViewBag.TransactionTypeList = await _utils.GetTransactionType();
        ViewBag.HeadofAccount_FiveList = await _utils.GetHeadofAccount_FiveOnlyCashandBank();

        return PartialView("~/Views/MasterInfo/ApprovalsRequest/DetailsProcessTypeApproval.cshtml", model);
      }
      if (processTypeID == 18)
      {
        var Vouchers = await _appDBContext.FI_Vouchers
                  .Include(v => v.VoucherDetails)
                  .FirstOrDefaultAsync(v => v.VoucherID == transactionID);

        if (Vouchers == null)
        {
          return NotFound();
        }

        Vouchers.VoucherDetails.Add(new FI_VoucherDetail() { VoucherID = Vouchers.VoucherID });

        var model = new PaymentVoucherIndexViewModel
        {
          Vouchers = Vouchers
        };

        ViewBag.VoucherTypeList = await _utils.GetVoucherType_Journal();
        ViewBag.TransactionTypeList = await _utils.GetTransactionType();
        ViewBag.HeadofAccount_FiveList = await _utils.GetHeadofAccount_Five();

        return PartialView("~/Views/MasterInfo/ApprovalsRequest/DetailsProcessTypeApproval.cshtml", model);
      }
      if (processTypeID == 19)
      {
        var Vouchers = await _appDBContext.FI_Vouchers
                  .Include(v => v.VoucherDetails)
                  .FirstOrDefaultAsync(v => v.VoucherID == transactionID);

        if (Vouchers == null)
        {
          return NotFound();
        }

        Vouchers.VoucherDetails.Add(new FI_VoucherDetail() { VoucherID = Vouchers.VoucherID });

        var model = new ReceivedVoucherIndexViewModel
        {
          Vouchers = Vouchers
        };

        ViewBag.VoucherTypeList = await _utils.GetVoucherType_Journal();
        ViewBag.TransactionTypeList = await _utils.GetTransactionType();
        ViewBag.HeadofAccount_FiveList = await _utils.GetHeadofAccount_Five();

        return PartialView("~/Views/MasterInfo/ApprovalsRequest/DetailsProcessTypeApproval.cshtml", model);
      }
      if (processTypeID == 20)
      {
        var MaterialRequisitions = await _appDBContext.ST_MaterialRequisitions
          .Include(v => v.MaterialRequisitionDetails)
          .FirstOrDefaultAsync(v => v.RequisitionID == transactionID);

        if (MaterialRequisitions == null)
        {
          return NotFound();
        }

        // Check RequisitionStatusTypeID
        if (MaterialRequisitions.RequisitionStatusTypeID != 1)
        {
          await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "After approval, editing is not allowed.....");

        }

        MaterialRequisitions.MaterialRequisitionDetails.Add(new ST_MaterialRequisitionDetail()
        {
          RequisitionID = MaterialRequisitions.RequisitionID
        });

        var model = new MaterialRequisitionsIndexViewModel
        {
          MaterialRequisitions = MaterialRequisitions
        };

        ViewBag.ItemList = await _utils.GetItemList();
        ViewBag.ItemNameList = await _utils.GetItemList();
        return PartialView("~/Views/MasterInfo/ApprovalsRequest/DetailsProcessTypeApproval.cshtml", model);
      }
      if (processTypeID == 21)
      {
        var purchaseRequestList = await _appDBContext.PR_PurchaseRequests
          .Where(v => v.PurchaseRequestID == transactionID)
          .ToListAsync(); // Fetch all items related to the ID

        if (purchaseRequestList == null || !purchaseRequestList.Any())
        {
          return NotFound();
        }

        ViewBag.ItemList = await _utils.GetItemList();
        ViewBag.ItemNameList = await _utils.GetItemList();
        ViewBag.ItemUnitList = await _utils.GetItemUnits();
        ViewBag.PriorityLevelList = await _utils.GetPriorityLevel();

        return PartialView("~/Views/MasterInfo/ApprovalsRequest/DetailsProcessTypeApproval.cshtml", purchaseRequestList);
      }
      if (processTypeID == 22)
      {
        var purchaseRequestList = await _appDBContext.PR_PurchaseRequests
          .Where(v => v.PurchaseRequestID == transactionID)
          .ToListAsync(); // Fetch all items related to the ID

        if (purchaseRequestList == null || !purchaseRequestList.Any())
        {
          return NotFound();
        }

        ViewBag.ItemList = await _utils.GetItemList();
        ViewBag.ItemNameList = await _utils.GetItemList();
        ViewBag.ItemUnitList = await _utils.GetItemUnits();
        ViewBag.PriorityLevelList = await _utils.GetPriorityLevel();

        return PartialView("~/Views/MasterInfo/ApprovalsRequest/DetailsProcessTypeApproval.cshtml", purchaseRequestList);
      }
      if (processTypeID == 23)
      {
        var PurchaseRequests = await _appDBContext.PR_PurchaseRequests
           .Include(v => v.Item)
           .Include(v => v.RequestStatusType)
           .FirstOrDefaultAsync(v => v.PurchaseRequestID == id);

        var RequestForQuotationList = await _appDBContext.PR_RequestForQuotations
            .FirstOrDefaultAsync(v => v.PurchaseRequestID == id);

        var CostComparisonList = await _appDBContext.PR_CostComparison
            .FirstOrDefaultAsync(v => v.PurchaseRequestID == id);


        if (PurchaseRequests == null)
        {
          return NotFound();
        }

        if (RequestForQuotationList == null)
        {
          return NotFound();
        }

        ViewBag.ItemList = await _utils.GetItemList();
        ViewBag.ItemNameList = await _utils.GetItemList();
        ViewBag.ItemUnitList = await _utils.GetItemUnits();
        ViewBag.PriorityLevelList = await _utils.GetPriorityLevel();
        ViewBag.VendorList = await _utils.GetVendorList();

        if (RequestForQuotationList != null)
        {
          ViewBag.QuotationVendorID1 = RequestForQuotationList.QuotationVendorID1;
          ViewBag.QuotationVendorID2 = RequestForQuotationList.QuotationVendorID2;
          ViewBag.QuotationVendorID3 = RequestForQuotationList.QuotationVendorID3;
          ViewBag.VendorListbyComparison = await _utils.GetVendorListbyComparison(RequestForQuotationList.PurchaseRequestID);
        }

        var model = new PurchaseRequestwithCostComparisonViewModel
        {
          PurchaseRequests = new List<PR_PurchaseRequest> { PurchaseRequests },
          CostComparisons = new List<PR_CostComparison> { CostComparisonList },
        };
        return PartialView("~/Views/MasterInfo/ApprovalsRequest/DetailsProcessTypeApproval.cshtml", model);
      }
      // Fallback view if no ProcessTypeID matches
      return View("~/Views/MasterInfo/ApprovalsRequest/ApprovalsRequest.cshtml", result);
    }
    private async Task<MonthlySalarySheetViewModel> GetMonthlySalarySheetAsync(string employeeId, int month, int year)
    {
      var salarySheet = new MonthlySalarySheetViewModel
      {
        EmployeeID = int.Parse(employeeId)
      };
      decimal sumSalary = 0;
      decimal sumAdditionalAllowance = 0;
      decimal sumOverTime = 0;
      decimal sumDeduction = 0;
      decimal sumFixedDeduction = 0;

      var connectionString = _configuration.GetConnectionString("AppDb");
      using (var connection = new SqlConnection(connectionString))
      {
        await connection.OpenAsync();

        var commandText = "EXEC GetMonthlySalarySheet @EmployeeID, @Month, @Year;";
        using (var command = new SqlCommand(commandText, connection))
        {
          command.Parameters.AddWithValue("@EmployeeID", employeeId);
          command.Parameters.AddWithValue("@Month", month);
          command.Parameters.AddWithValue("@Year", year);

          using (var reader = await command.ExecuteReaderAsync())
          {
            while (await reader.ReadAsync())
            {
              for (int i = 0; i < reader.FieldCount; i++)
              {
                string columnName = reader.GetName(i);

                // Handle nullable or non-nullable values
                var value = reader.IsDBNull(i) ? "0" : reader.GetValue(i).ToString();

                if (columnName == "EmployeeName")
                {
                  salarySheet.EmployeeName = value;
                }
                if (columnName == "BranchID")
                {
                  salarySheet.BranchID = int.Parse(value);
                }
                if (columnName == "MonthID")
                {
                  salarySheet.MonthID = int.Parse(value);
                }
                if (columnName == "MonthName")
                {
                  salarySheet.MonthName = value;
                }
                if (columnName == "Year")
                {
                  salarySheet.Year = int.Parse(value);
                }

                // Process each column by category
                if (columnName.StartsWith("Salary_"))
                {
                  salarySheet.SalaryDetails.Add(columnName.Split('_')[1], decimal.Parse(value));
                  sumSalary += decimal.Parse(value);
                }
                else if (columnName.StartsWith("AdditionalAllowance_"))
                {
                  salarySheet.AdditionalAllowances.Add(columnName.Split('_')[1], decimal.Parse(value));
                  sumAdditionalAllowance += decimal.Parse(value);
                }
                else if (columnName.StartsWith("OverTime_"))
                {
                  salarySheet.OvertimeDetails.Add(columnName.Split('_')[1], decimal.Parse(value));
                  sumOverTime += decimal.Parse(value);
                }
                else if (columnName.StartsWith("Deduction_"))
                {
                  salarySheet.Deductions.Add(columnName.Split('_')[1], decimal.Parse(value));
                  sumDeduction += decimal.Parse(value);
                }
                else if (columnName.StartsWith("FixedDeduction_"))
                {
                  salarySheet.FixedDeductions.Add(columnName.Split('_')[1], decimal.Parse(value));
                  sumFixedDeduction += decimal.Parse(value);
                }
              }
            }
          }
        }
      }
      salarySheet.SumSalary = sumSalary;
      salarySheet.SumAdditionalAllowance = sumAdditionalAllowance;
      salarySheet.SumOverTime = sumOverTime;
      salarySheet.SumDeduction = sumDeduction;
      salarySheet.SumFixedDeduction = sumFixedDeduction;

      salarySheet.DeservedAmount = (sumSalary + sumAdditionalAllowance + sumOverTime) - (sumDeduction + sumFixedDeduction);

      return salarySheet;
    }
  }
}

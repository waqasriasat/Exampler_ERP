using Exampler_ERP.Models;
using Exampler_ERP.Models.Temp;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Exampler_ERP.Controllers.MasterInfo
{
  public class ApprovalsRequestController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    public ApprovalsRequestController(AppDBContext appDBContext, IConfiguration configuration, Utils utils)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
    }
    
    public async Task<IActionResult> Index(int? id = null)
    {
      // Base query: filter for records where AppID == 0
      var query = _appDBContext.CR_ProcessTypeApprovalDetails
          .Include(pta => pta.CR_ProcessTypeApproval)
              .ThenInclude(pta => pta.Employee)
          .Include(pta => pta.CR_ProcessTypeApproval)
              .ThenInclude(pta => pta.ProcessType)
          .Include(pta => pta.ProcessTypeApprovalDetailDoc)
          .Where(pta => pta.AppID == 0);

      // If an id is provided, further filter by id
      if (id.HasValue)
      {
        query = query.Where(pta => pta.CR_ProcessTypeApproval.ProcessTypeID == id.Value);
      }

      // Execute the query and order the results
      var result = await query
          .OrderByDescending(pta => pta.ApprovalProcessDetailID)
          .ToListAsync();

      // Return the view with the filtered results
      return View("~/Views/MasterInfo/ApprovalsRequest/ApprovalsRequest.cshtml", result);
    }

    [HttpGet]
    public async Task<IActionResult> DownloadDocument(int id)
    {
      var document = await _appDBContext.CR_ProcessTypeApprovalDetailDocs
          .FirstOrDefaultAsync(d => d.ApprovalProcessDetailDocID == id);

      if (document == null || document.Doc == null)
      {
        return NotFound();
      }

      return File(document.Doc, "application/octet-stream", document.DocName);
    }
    [HttpGet]
    public async Task<IActionResult> ViewDocument(int id)
    {
      // Retrieve the document based on ApprovalProcessDetailDocID
      var document = await _appDBContext.CR_ProcessTypeApprovalDetailDocs
          .FirstOrDefaultAsync(d => d.ApprovalProcessDetailDocID == id);

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
    .Where(pta => pta.AppID == 1 && pta.ApprovalProcessID == id)
    .OrderBy(pta => pta.Rank) 
    .ToListAsync();

      ViewBag.EmployeesList = await _utils.GetEmployee();
      ViewBag.RolesList = await _utils.GetRoles();


      if (result == null)
      {
        return NotFound($"No approval record found for ApprovalProcessID = {id}");
      }

      return PartialView("~/Views/MasterInfo/ApprovalsRequest/ApprovalsProcessTypeApproval.cshtml", result);
    }
    public async Task<IActionResult> Actions(int id)
    {
      var result = await _appDBContext.CR_ProcessTypeApprovalDetails
               .Where(pta => pta.ApprovalProcessDetailID == id)
               .Include(pta => pta.CR_ProcessTypeApproval)
               .FirstOrDefaultAsync();

      var ProcessTypeID = result?.CR_ProcessTypeApproval?.ProcessTypeID;
      var TransactionID = result?.CR_ProcessTypeApproval?.TransactionID;
      var RankID = result?.Rank;
      var ApprovalProcessDetailID = result?.ApprovalProcessDetailID;

      ViewBag.ProcessTypeID = ProcessTypeID;
      ViewBag.TransactionID = TransactionID;
      ViewBag.RankID = RankID;
      ViewBag.ApprovalProcessDetailID = ApprovalProcessDetailID;


      if (result == null)
      {
        return NotFound($"No approval record found for ApprovalProcessID = {id}");
      }

      return PartialView("~/Views/MasterInfo/ApprovalsRequest/ActionsProcessTypeApproval.cshtml", result);
    }

    [HttpPost]
    public async Task<IActionResult> Approved(CR_ProcessTypeApprovalDetail processTypeApprovalDetail, IFormFile FileUpload, int ProcessTypeID, int TransactionID)
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
              ApprovalProcessDetailID = processTypeApprovalDetail.ApprovalProcessDetailID,
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
            ApprovalProcessID = processTypeApprovalDetail.ApprovalProcessID,
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
              user.ApprovalProcessID = processTypeApprovalDetail.ApprovalProcessID;

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
              employee.ApprovalProcessID = processTypeApprovalDetail.ApprovalProcessID;

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
              contract.ApprovalProcessID = processTypeApprovalDetail.ApprovalProcessID;

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
              contractRenewal.ApprovalProcessID = processTypeApprovalDetail.ApprovalProcessID;
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
              endOfService.ApprovalProcessID = processTypeApprovalDetail.ApprovalProcessID;
              await _appDBContext.SaveChangesAsync();
            }

            var contractToUpdate = _appDBContext.HR_Contracts
                                               .FirstOrDefault(c => c.EmployeeID == endOfService.EmployeeID);

            if (contractToUpdate != null)
            {
              contractToUpdate.ActiveYNID = 0;
              _appDBContext.SaveChanges();
            }

            var employee = await _appDBContext.HR_Employees
                                              .FirstOrDefaultAsync(e => e.EmployeeID == endOfService.EmployeeID);

            if (employee != null)
            {
              employee.EmployeeStatusTypeID = 5; // End of Service status
              employee.ActiveYNID = 0; // Deactivate employee
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
              salary.ApprovalProcessID = processTypeApprovalDetail.ApprovalProcessID;

              _appDBContext.Update(salary);
              await _appDBContext.SaveChangesAsync();
            }
          }
          if (ProcessTypeID == 7)
          {
            var addionalAllowance = await _appDBContext.HR_AddionalAllowances
                                                     .Where(u => u.AddionalAllowanceID == transactionID)
                                                     .FirstOrDefaultAsync();


            if (addionalAllowance != null)
            {
              addionalAllowance.FinalApprovalID = 1;
              addionalAllowance.ApprovalProcessID = processTypeApprovalDetail.ApprovalProcessID;

              _appDBContext.Update(addionalAllowance);
              await _appDBContext.SaveChangesAsync();
            }
          }
          if (ProcessTypeID == 8)
          {

          }
          if (ProcessTypeID == 9)
          {

          }
          if (ProcessTypeID == 10)
          {
            var fixedDeduction = await _appDBContext.HR_FixedDeductions
                                                     .Where(u => u.FixedDeductionID == transactionID)
                                                     .FirstOrDefaultAsync();


            if (fixedDeduction != null)
            {
              fixedDeduction.FinalApprovalID = 1;
              fixedDeduction.ApprovalProcessID = processTypeApprovalDetail.ApprovalProcessID;

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
              vacation.ApprovalProcessID = processTypeApprovalDetail.ApprovalProcessID;

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
              vacationSettle.ApprovalProcessID = processTypeApprovalDetail.ApprovalProcessID;

              _appDBContext.Update(vacationSettle);
              await _appDBContext.SaveChangesAsync();
            }
          }
          if (ProcessTypeID == 13)
          {

          }
          if (ProcessTypeID == 14)
          {
            var EmployeeRequest = await _appDBContext.HR_EmployeeRequests
                                                     .Where(u => u.EmployeeRequestID == transactionID)
                                                     .FirstOrDefaultAsync();


            if (EmployeeRequest != null)
            {
              EmployeeRequest.FinalApprovalID = 1;
              EmployeeRequest.ApprovalProcessID = processTypeApprovalDetail.ApprovalProcessID;

              _appDBContext.Update(EmployeeRequest);
              await _appDBContext.SaveChangesAsync();
            }
          }
        }
        return Json(new { success = true });
      }

      return PartialView("~/Views/MasterInfo/ApprovalsRequest/ActionsProcessTypeApproval.cshtml", processTypeApprovalDetail);
    }
    [HttpPost]
    public async Task<IActionResult> Reject(CR_ProcessTypeApprovalDetail processTypeApprovalDetail, IFormFile FileUpload, int ProcessTypeID, int TransactionID)
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
              ApprovalProcessDetailID = processTypeApprovalDetail.ApprovalProcessDetailID,
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
              user.ActiveYNID = 0;
              user.FinalApprovalID = 2;
              user.ApprovalProcessID = processTypeApprovalDetail.ApprovalProcessID;

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
            employee.ActiveYNID = 0;
            employee.FinalApprovalID = 2;
            employee.ApprovalProcessID = processTypeApprovalDetail.ApprovalProcessID;

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
            contract.ActiveYNID = 0;
            contract.FinalApprovalID = 2;
            contract.ApprovalProcessID = processTypeApprovalDetail.ApprovalProcessID;

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
              contractRenewal.ApprovalProcessID = processTypeApprovalDetail.ApprovalProcessID;

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
              endOfService.ApprovalProcessID = processTypeApprovalDetail.ApprovalProcessID;

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
              salary.ApprovalProcessID = processTypeApprovalDetail.ApprovalProcessID;

              _appDBContext.Update(salary);
              await _appDBContext.SaveChangesAsync();
            }
          }
          if (ProcessTypeID == 7)
          {

          }
          if (ProcessTypeID == 8)
          {
            var addionalAllowance = await _appDBContext.HR_AddionalAllowances
                                                     .Where(u => u.AddionalAllowanceID == transactionID)
                                                     .FirstOrDefaultAsync();


            if (addionalAllowance != null)
            {
              addionalAllowance.FinalApprovalID = 2;
              addionalAllowance.ApprovalProcessID = processTypeApprovalDetail.ApprovalProcessID;

              _appDBContext.Update(addionalAllowance);
              await _appDBContext.SaveChangesAsync();
            }
          }
          if (ProcessTypeID == 9)
          {

          }
          if (ProcessTypeID == 10)
          {
            var fixedDeduction = await _appDBContext.HR_FixedDeductions
                                                     .Where(u => u.FixedDeductionID == transactionID)
                                                     .FirstOrDefaultAsync();


            if (fixedDeduction != null)
            {
              fixedDeduction.FinalApprovalID = 2;
              fixedDeduction.ApprovalProcessID = processTypeApprovalDetail.ApprovalProcessID;

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
              vacation.ApprovalProcessID = processTypeApprovalDetail.ApprovalProcessID;

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
              vacationSettle.ApprovalProcessID = processTypeApprovalDetail.ApprovalProcessID;

              _appDBContext.Update(vacationSettle);
              await _appDBContext.SaveChangesAsync();
            }
          }
          if (ProcessTypeID == 13)
          {

          }
          if (ProcessTypeID == 14)
          {
            var EmployeeRequest = await _appDBContext.HR_EmployeeRequests
                                                     .Where(u => u.EmployeeRequestID == transactionID)
                                                     .FirstOrDefaultAsync();


            if (EmployeeRequest != null)
            {
              EmployeeRequest.FinalApprovalID = 2;
              EmployeeRequest.ApprovalProcessID = processTypeApprovalDetail.ApprovalProcessID;

              _appDBContext.Update(EmployeeRequest);
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
          .Where(pta => pta.ApprovalProcessID == id)
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

      }
      if (processTypeID == 14)
      {
        ViewBag.EmployeeRequestTypeList = await _utils.GetEmployeeRequestTypes();
        var EmployeeRequests = await _appDBContext.HR_EmployeeRequests
        .Where(v => v.EmployeeRequestID == transactionID)
                                     .Include(c => c.Employee)
                                      .Include(c => c.Settings_EmployeeRequestType)
                                     .FirstOrDefaultAsync();
        return PartialView("~/Views/MasterInfo/ApprovalsRequest/DetailsProcessTypeApproval.cshtml", EmployeeRequests);
      }
      // Fallback view if no ProcessTypeID matches
      return View("~/Views/MasterInfo/ApprovalsRequest/ApprovalsRequest.cshtml", result);
    }



  }
}

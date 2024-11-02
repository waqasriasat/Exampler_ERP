using Exampler_ERP.Models;
using Exampler_ERP.Models.Temp;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Contracts;

namespace Exampler_ERP.Controllers.HR.Employeement
{
  public class ContractRenewalController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;

    public ContractRenewalController(AppDBContext appDBContext, IConfiguration configuration, Utils utils)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
    }

    public async Task<IActionResult> Index(int? id)
    {
      var today = DateTime.Today;
      var futureDate = today.AddDays(30);

      var employeecontractQuery = _appDBContext.HR_Contracts
          .Where(c => c.DeleteYNID != 1 &&
                      c.ContractTypeID == 1 &&
                      c.EndDate != null &&
                      (c.EndDate.Value <= futureDate || c.EndDate.Value < today));

      if (id.HasValue)
      {
        employeecontractQuery = employeecontractQuery.Where(c => c.EmployeeID == id.Value);
      }
      var contracts = await employeecontractQuery
          .Include(c => c.Employee)
          .ToListAsync();

      return View("~/Views/HR/Employeement/ContractRenewal/ContractRenewal.cshtml", contracts);
    }
    public async Task<IActionResult> Edit(int id)
    {
      var contractRenewal = await _appDBContext.HR_ContractRenewals
        .Where(c => c.ContractID == id)
        .FirstOrDefaultAsync();
      

      var contract = await _appDBContext.HR_Contracts
       .Where(c => c.ContractID == id)
       .Select(c => new { c.StartDate, c.EndDate })
       .FirstOrDefaultAsync();

      // Pass the contract data to the view using ViewBag
      ViewBag.ContractID = id;
      ViewBag.PStartDate = contract?.StartDate;
      ViewBag.PEndDate = contract?.EndDate;
      ViewBag.EmployeesList = await _utils.GetEmployee();
      if (contractRenewal == null)
      {
        return PartialView("~/Views/HR/Employeement/ContractRenewal/EditContractRenewal.cshtml", new HR_ContractRenewal());
      }

      return PartialView("~/Views/HR/Employeement/ContractRenewal/EditContractRenewal.cshtml", contract);
    }


    [HttpPost]
    public async Task<IActionResult> Edit(DateTime NStartDate, DateTime NEndDate, DateTime PStartDate, DateTime PEndDate, Models.HR_ContractRenewal model)
    {
      if (ModelState.IsValid)
      {
        // Insert the new ContractRenewal record
        var contractRenewal = new HR_ContractRenewal
        {
          ContractID = model.ContractID,
          PStartDate = PStartDate,
          PEndDate = PEndDate,
          NStartDate = model.NStartDate,
          NEndDate = model.NEndDate,
          FinalApprovalID = 0,
          ProcessTypeApprovalID = 0
        };

        _appDBContext.HR_ContractRenewals.Add(contractRenewal);
        await _appDBContext.SaveChangesAsync();

        var contractRenewalID = contractRenewal.ContractRenewalID;
        if (contractRenewalID > 0)
        {
          var processCount = await _appDBContext.CR_ProcessTypeApprovalSetups
                              .Where(pta => pta.ProcessTypeID > 0 && pta.ProcessTypeID == 4)
                              .CountAsync();

          var getEmployeeID = await _appDBContext.HR_Contracts
                              .Where(pta => pta.ContractID == contractRenewal.ContractID)
                              .FirstOrDefaultAsync();

          if (processCount > 0)
          {
            var newProcessTypeApproval = new CR_ProcessTypeApproval
            {
              ProcessTypeID = 4,
              Notes = "Contract Renewal",
              Date = DateTime.Now,
              EmployeeID = getEmployeeID.EmployeeID,
              UserID = HttpContext.Session.GetInt32("UserID") ?? default(int),
              TransactionID = contractRenewalID
            };

            _appDBContext.CR_ProcessTypeApprovals.Add(newProcessTypeApproval);
            await _appDBContext.SaveChangesAsync();

            var nextApprovalSetup = await _appDBContext.CR_ProcessTypeApprovalSetups
                                        .Where(pta => pta.ProcessTypeID == 4 && pta.Rank == 1)
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
            var contractToUpdate = _appDBContext.HR_Contracts
                                                .FirstOrDefault(c => c.ContractID == contractRenewal.ContractID);

            if (contractToUpdate != null)
            {
              contractToUpdate.EndDate = contractRenewal.NEndDate;

              _appDBContext.SaveChanges();
            }
            await _appDBContext.SaveChangesAsync();
            TempData["SuccessMessage"] = "Contract Renewal Created successfully. No process setup found, Contract activated.";
            return Json(new { success = true});
          }
        }
        TempData["SuccessMessage"] = "Contract Renewal Created successfully. Continue to the Approval Process Setup for Contract Activation.";
        return Json(new { success = true });
      }
      TempData["ErrorMessage"] = "Error creating Contract Renewal. Please check the inputs.";
      return PartialView("~/Views/HR/Employeement/ContractRenewal/EditContractRenewal.cshtml", model);
    }

  }
}

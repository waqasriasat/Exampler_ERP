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
      var contract = await _appDBContext.HR_Contracts.FindAsync(id);
      if (contract == null)
      {
        return NotFound();
      }

      ViewBag.EmployeesList = await _utils.GetEmployee();
 
      return PartialView("~/Views/HR/Employeement/ContractRenewal/EditContractRenewal.cshtml",contract);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(DateTime NewStartDate, DateTime NewEndDate, Models.HR_Contract model)
    {
      if (ModelState.IsValid)
      {
        // Insert the new ContractRenewal record
        var contractRenewal = new HR_ContractRenewal
        {
          ContractID = model.ContractID,
          PStartDate = model.StartDate,
          PEndDate = model.EndDate,
          NStartDate = NewStartDate,
          NEndDate = NewEndDate,
          FinalApprovalID = 0,
          ApprovalProcessID = 0
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
                ApprovalProcessID = newProcessTypeApproval.ApprovalProcessID,
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
            return Json(new { success = true, message = "No process setup found, User activated." });
          }
        }

        return Json(new { success = true });
      }

      return PartialView("~/Views/HR/Employeement/ContractRenewal/EditContractRenewal.cshtml", model);
    }

  }
}

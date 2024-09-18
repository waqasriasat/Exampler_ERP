using Exampler_ERP.Models;
using Exampler_ERP.Models.Temp;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

    public async Task<IActionResult> Index()
    {
      var today = DateTime.Today;
      var futureDate = today.AddDays(30);

      var contracts = await _appDBContext.HR_Contracts
          .Where(c => c.DeleteYNID != 1 &&
                      c.ContractTypeID == 1 &&
                      c.EndDate != null &&
                      (c.EndDate.Value <= futureDate || c.EndDate.Value < today))
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

        return Json(new { success = true });
      }

      return PartialView("~/Views/HR/Employeement/ContractRenewal/EditContractRenewal.cshtml", model);
    }

  }
}

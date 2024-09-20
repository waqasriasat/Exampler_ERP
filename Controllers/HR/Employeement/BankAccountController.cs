using Exampler_ERP.Models.Temp;
using Exampler_ERP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Utilities;

namespace Exampler_ERP.Controllers.HR.Employeement
{
  public class BankAccountController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly ILogger<BankAccountController> _logger;
    private readonly Utils _utils;
    public BankAccountController(AppDBContext appDBContext, IConfiguration configuration, ILogger<BankAccountController> logger, Utils utils)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _logger = logger;
      _utils = utils;
    }
    public async Task<IActionResult> Index()
    {
      var employeeBankAccounts = await (from emp in _appDBContext.HR_Employees
                                        join con in _appDBContext.HR_Contracts
                                        on emp.EmployeeID equals con.EmployeeID
                                        join bank in _appDBContext.HR_BankAccounts
                                        on emp.EmployeeID equals bank.EmployeeID into joinGroup
                                        from j in joinGroup.DefaultIfEmpty()
                                        where con.ActiveYNID == 1 && emp.ActiveYNID == 1
                                        select new
                                        {
                                          emp.EmployeeID,
                                          emp.FirstName,
                                          emp.FatherName,
                                          emp.FamilyName,
                                          BankName = j != null ? j.BankName : "N/A"
                                        }).ToListAsync();

      var employeeCounts = employeeBankAccounts.Select(ej => new EmployeeBankAccountViewModel
      {
        EmployeeID = ej.EmployeeID,
        EmployeeName = $"{ej.FirstName} {ej.FatherName} {ej.FamilyName}",
        BankName = ej.BankName
      }).ToList();

      var viewModel = new EmployeeBankAccountListViewModel
      {
        EmployeeBankAccount = employeeCounts
      };

      return View("~/Views/HR/Employeement/BankAccount/BankAccount.cshtml", viewModel);
    }


    public async Task<IActionResult> Edit(int id)
    {
      var existingBankAccount = await _appDBContext.HR_BankAccounts
          .Where(pt => pt.EmployeeID == id)
          .Include(pt => pt.Employee)
          .FirstOrDefaultAsync();

      if (existingBankAccount != null)
      {
        var BankAccounts = new List<HR_BankAccount> { existingBankAccount };
        return PartialView("~/Views/HR/Employeement/BankAccount/EditBankAccount.cshtml", BankAccounts);
      }
      else
      {
        var employee = await _appDBContext.HR_Employees
            .Where(p => p.EmployeeID == id)
            .FirstOrDefaultAsync();

        if (employee != null)
        {
          var newBankAccount = new HR_BankAccount
          {
            EmployeeID = employee.EmployeeID,
            Employee = employee,
            AccountHolderName = "",
            AccountNumber = "",
            BankName = "", 
            BranchName = "", 
            AccountType = "", 
            BankContact = "", 
            BankAddress = "" 
          };

          var BankAccounts = new List<HR_BankAccount> { newBankAccount };

          return PartialView("~/Views/HR/Employeement/BankAccount/EditBankAccount.cshtml", BankAccounts);
        }
        else
        {
          return NotFound(); // Employee not found
        }
      }
    }



    [HttpPost]
    public async Task<IActionResult> Edit(HR_BankAccount updatedBankAccount)
    {
      if (ModelState.IsValid)
      {
        try
        {
          var existingBankAccount = await _appDBContext.HR_BankAccounts
              .Where(j => j.EmployeeID == updatedBankAccount.EmployeeID)
              .FirstOrDefaultAsync();
          if (existingBankAccount != null)
          {
            existingBankAccount.AccountHolderName = updatedBankAccount.AccountHolderName;
            existingBankAccount.AccountNumber = updatedBankAccount.AccountNumber;
            existingBankAccount.BankName = updatedBankAccount.BankName;
            existingBankAccount.BranchName = updatedBankAccount.BranchName;
            existingBankAccount.AccountType = updatedBankAccount.AccountType;
            existingBankAccount.BankContact = updatedBankAccount.BankContact;
            existingBankAccount.BankAddress = updatedBankAccount.BankAddress;
            _appDBContext.HR_BankAccounts.Update(existingBankAccount);
          }
          else
          {
            _appDBContext.HR_BankAccounts.Add(updatedBankAccount);

          }
          await _appDBContext.SaveChangesAsync();
          return Json(new { success = true });
        }
        catch (Exception ex)
        {
          _logger.LogError(ex, "Error saving BankAccount changes");
          return Json(new { success = false, message = "An error occurred while saving changes." });
        }
      }
      return Json(new { success = false, message = "Invalid model state." });
    }
  }
}

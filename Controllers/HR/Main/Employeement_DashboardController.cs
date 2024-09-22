using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Exampler_ERP.Controllers.HR.Main
{
  public class Employeement_DashboardController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;


    public Employeement_DashboardController(AppDBContext appDBContext, IConfiguration configuration, Utils utils)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
    }
    public async Task<IActionResult> Index()
    {
      ViewBag.ApplicantCount = await _appDBContext.HR_Applicants.CountAsync();
      ViewBag.EmployeeCount = await _appDBContext.HR_Employees.CountAsync();
      ViewBag.ContractCount = await _appDBContext.HR_Contracts.CountAsync();
      ViewBag.SalaryCount = await _appDBContext.HR_Salarys.CountAsync();
      ViewBag.JoiningCount = await _appDBContext.HR_Joinings.CountAsync();
      ViewBag.BankAccountCount = await _appDBContext.HR_BankAccounts.CountAsync();
      ViewBag.ContractRenewwalCount = await _appDBContext.HR_ContractRenewals.CountAsync();
      //ViewBag.LeaveBalanceCount = await _appDBContext.leave.CountAsync();
      //ViewBag.CardPrintCount = await _appDBContext.Card.CountAsync();
    
      return View("~/Views/HR/Main/Dashboard/Employeement_Dashboard.cshtml");
    }
  }
}
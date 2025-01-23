using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Exampler_ERP.Controllers.StoreManagement.Main
{
  public class ST_StoreManagement_DashboardController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;


    public ST_StoreManagement_DashboardController(AppDBContext appDBContext, IConfiguration configuration, Utils utils)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
    }
    public async Task<IActionResult> Index()
    {
      int EmployeeID = int.Parse(HttpContext.Session.GetInt32("UserID")?.ToString() ?? "0");
      ViewBag.MaterialRequisitionCount = await _appDBContext.ST_MaterialRequisitions
          .Where(v => v.EmployeeID == EmployeeID)
          .CountAsync();
      ViewBag.MaterialReceivedCount = await _appDBContext.ST_MaterialReceiveds.CountAsync();
      ViewBag.StockCount = await _appDBContext.ST_Stocks.CountAsync();
      ViewBag.MaterialIssuanceCount = await _appDBContext.ST_MaterialIssuances.CountAsync();
      //ViewBag.JoiningCount = await _appDBContext.HR_Joinings.CountAsync();
      //ViewBag.BankAccountCount = await _appDBContext.HR_BankAccounts.CountAsync();
      //ViewBag.ContractRenewwalCount = await _appDBContext.HR_ContractRenewals.CountAsync();
      //ViewBag.LeaveBalanceCount = await _appDBContext.leave.CountAsync();
      //ViewBag.CardPrintCount = await _appDBContext.Card.CountAsync();

      return View("~/Views/StoreManagement/Main/Dashboard/ST_StoreManagement_Dashboard.cshtml");
    }
  }
}

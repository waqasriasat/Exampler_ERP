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
      int UserID = int.Parse(HttpContext.Session.GetInt32("UserID").ToString());
      int departmentID = await _utils.PostUserIDGetDepartmentID(UserID);
      ViewBag.MaterialRequisitionCount = await _appDBContext.ST_MaterialRequisitions
          .Where(v => v.DepartmentTypeID == departmentID)
          .CountAsync();
      ViewBag.MaterialReceivedCount = await _appDBContext.ST_MaterialReceiveds.CountAsync();
      ViewBag.StockCount = await _appDBContext.ST_Stocks.CountAsync();
      ViewBag.MaterialIssuanceCount = await _appDBContext.ST_MaterialIssuances.CountAsync();
      ViewBag.ProcurementQueueCount = await _appDBContext.PR_ProcurementQueues
    .Include(d => d.Item)
    .Where(d => d.ForwardYNID == 0)
    .CountAsync(); 
      //ViewBag.BankAccountCount = await _appDBContext.HR_BankAccounts.CountAsync();
      //ViewBag.ContractRenewwalCount = await _appDBContext.HR_ContractRenewals.CountAsync();
      //ViewBag.LeaveBalanceCount = await _appDBContext.leave.CountAsync();
      //ViewBag.CardPrintCount = await _appDBContext.Card.CountAsync();

      return View("~/Views/StoreManagement/Main/Dashboard/ST_StoreManagement_Dashboard.cshtml");
    }
  }
}

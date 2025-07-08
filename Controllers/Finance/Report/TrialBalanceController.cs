using Exampler_ERP.Controllers.Setup;
using Exampler_ERP.Models;
using Exampler_ERP.Models.Temp;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Web.Helpers;

namespace Exampler_ERP.Controllers.Finance.Report
{
  public class TrialBalanceController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
private readonly IHubContext<NotificationHub> _hubContext;

    public TrialBalanceController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
_hubContext = hubContext;
 
    }
    public async Task<IActionResult> Index()
    {
      ViewBag.HeadofAccount_FiveList = await _utils.GetHeadofAccount_Five();
      return View("~/Views/Finance/Report/TrialBalance/TrialBalance.cshtml", new FI_Voucher());
    }
    [HttpPost]
    public async Task<IActionResult> Print([FromBody] TrialBalanceReportModel model)
    {
      if (model == null || model.Year == null)
      {
        return BadRequest("Invalid data received.");
      }

      DateTime startDate;
      DateTime endDate;

      if (model.Month.HasValue)
      {
        startDate = new DateTime(model.Year.Value, model.Month.Value, 1);
        endDate = startDate.AddMonths(1).AddDays(-1);
      }
      else
      {
        startDate = new DateTime(model.Year.Value, 1, 1);
        endDate = new DateTime(model.Year.Value, 12, 31);
      }

      var groupedVouchers = await _appDBContext.FI_VoucherDetails
          .Include(v => v.Voucher)
          .Include(v => v.HeadofAccount_Five)
          .Where(v => v.Voucher.VoucherDate >= startDate && v.Voucher.VoucherDate <= endDate)
          .GroupBy(v => v.HeadofAccount_Five.HeadofAccount_FiveName)
          .Select(g => new
          {
            AccountName = g.Key,
            TotalDebit = g.Sum(v => v.DrAmt ?? 0),
            TotalCredit = g.Sum(v => v.CrAmt ?? 0),
            TransactionCount = g.Count() // Count of transactions in the group
          })
          .ToListAsync();

      var viewModel = groupedVouchers.Select(g => new TrialBalanceReportViewModel
      {
        AccountName = g.AccountName,
        DrAmt = g.TotalDebit,
        CrAmt = g.TotalCredit,
        TransactionCount = g.TransactionCount
      }).ToList();

      ViewBag.Month = model.Month;
      ViewBag.Year = model.Year;

      return View("~/Views/Finance/Report/TrialBalance/PrintTrialBalance.cshtml", viewModel);
    }
  }
}

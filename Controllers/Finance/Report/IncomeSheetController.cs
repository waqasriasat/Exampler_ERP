using Exampler_ERP.Controllers.Setup;
using Exampler_ERP.Models;
using Exampler_ERP.Models.Temp;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Identity.Client;

namespace Exampler_ERP.Controllers.Finance.Report
{
  public class IncomeSheetController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
private readonly IHubContext<NotificationHub> _hubContext;

    public IncomeSheetController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
_hubContext = hubContext;
 
    }
    public async Task<IActionResult> Index()
    {
      ViewBag.HeadofAccount_FiveList = await _utils.GetHeadofAccount_Five();

      return View("~/Views/Finance/Report/IncomeSheet/IncomeSheet.cshtml", new FI_Voucher());
    }
    [HttpPost]
    public async Task<IActionResult> Print([FromBody] IncomeSheetReportModel model)
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
          .GroupBy(v => new
          {
            v.HeadofAccount_Five.HeadofAccount_FiveID, // AccountID
            v.HeadofAccount_Five.HeadofAccount_FiveName
          })
          .Select(g => new
          {
            AccountID = g.Key.HeadofAccount_FiveID,
            AccountName = g.Key.HeadofAccount_FiveName,
            TotalDebit = g.Sum(v => v.DrAmt ?? 0),
            TotalCredit = g.Sum(v => v.CrAmt ?? 0),
            TransactionCount = g.Count()
          })
          .ToListAsync();

      // Classify accounts as Revenue or Expense
      var revenues = new List<IncomeSheetReportViewModel>();
      var expenses = new List<IncomeSheetReportViewModel>();

      foreach (var account in groupedVouchers)
      {
        if (await IsRevenueAccountAsync(account.AccountID))
        {
          revenues.Add(new IncomeSheetReportViewModel
          {
            AccountID = account.AccountID,
            AccountName = account.AccountName,
            DrAmt = account.TotalDebit,
            CrAmt = account.TotalCredit,
            TransactionCount = account.TransactionCount
          });
        }
        else if (await IsExpenseAccountAsync(account.AccountID))
        {
          expenses.Add(new IncomeSheetReportViewModel
          {
            AccountID = account.AccountID,
            AccountName = account.AccountName,
            DrAmt = account.TotalDebit,
            CrAmt = account.TotalCredit,
            TransactionCount = account.TransactionCount
          });
        }
      }

      // Compute totals
      var totalRevenues = revenues.Sum(r => r.CrAmt + r.DrAmt);
      var totalExpenses = expenses.Sum(e => e.CrAmt + e.DrAmt);

      var incomeSheetViewModel = new IncomeSheetViewModel
      {
        Revenues = revenues,
        Expenses = expenses,
        TotalRevenues = totalRevenues,
        TotalExpenses = totalExpenses
      };

      ViewBag.Month = model.Month;
      ViewBag.Year = model.Year;

      return View("~/Views/Finance/Report/IncomeSheet/PrintIncomeSheet.cshtml", incomeSheetViewModel);
    }

    private async Task<bool> IsRevenueAccountAsync(int accountID)
    {
      var revenueAccounts = await (from shafi in _appDBContext.Settings_HeadofAccount_Fives
                                   join shafo in _appDBContext.Settings_HeadofAccount_Fours
                                       on shafi.HeadofAccount_FourID equals shafo.HeadofAccount_FourID
                                   join shat in _appDBContext.Settings_HeadofAccount_Thirds
                                       on shafo.HeadofAccount_ThirdID equals shat.HeadofAccount_ThirdID
                                   join shas in _appDBContext.Settings_HeadofAccount_Seconds
                                       on shat.HeadofAccount_SecondID equals shas.HeadofAccount_SecondID
                                   join shafir in _appDBContext.Settings_HeadofAccount_Firsts
                                       on shas.HeadofAccount_FirstID equals shafir.HeadofAccount_FirstID
                                   where shafir.HeadofAccount_FirstName == "Revenue"
                                   select shafi.HeadofAccount_FiveID)
                                   .ToListAsync();
      return revenueAccounts.Contains(accountID);
    }

    private async Task<bool> IsExpenseAccountAsync(int accountID)
    {
      var expenseAccounts = await (from shafi in _appDBContext.Settings_HeadofAccount_Fives
                                   join shafo in _appDBContext.Settings_HeadofAccount_Fours
                                       on shafi.HeadofAccount_FourID equals shafo.HeadofAccount_FourID
                                   join shat in _appDBContext.Settings_HeadofAccount_Thirds
                                       on shafo.HeadofAccount_ThirdID equals shat.HeadofAccount_ThirdID
                                   join shas in _appDBContext.Settings_HeadofAccount_Seconds
                                       on shat.HeadofAccount_SecondID equals shas.HeadofAccount_SecondID
                                   join shafir in _appDBContext.Settings_HeadofAccount_Firsts
                                       on shas.HeadofAccount_FirstID equals shafir.HeadofAccount_FirstID
                                   where shafir.HeadofAccount_FirstName == "Expenses"
                                   select shafi.HeadofAccount_FiveID)
                                   .ToListAsync();

      return expenseAccounts.Contains(accountID);
    }



  }
}

using Exampler_ERP.Controllers.Setup;
using Exampler_ERP.Models;
using Exampler_ERP.Models.Temp;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Exampler_ERP.Controllers.Finance.Report
{
  public class IncomeSheetController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    public IncomeSheetController(AppDBContext appDBContext, IConfiguration configuration, Utils utils)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
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
          .GroupBy(v => v.HeadofAccount_Five.HeadofAccount_FiveName)
          .Select(g => new
          {
            AccountName = g.Key,
            TotalDebit = g.Sum(v => v.DrAmt ?? 0),
            TotalCredit = g.Sum(v => v.CrAmt ?? 0),
            TransactionCount = g.Count()
          })
          .ToListAsync();

      var viewModel = groupedVouchers.Select(g => new IncomeSheetReportViewModel
      {
        AccountName = g.AccountName,
        DrAmt = g.TotalDebit,
        CrAmt = g.TotalCredit,
        TransactionCount = g.TransactionCount
      }).ToList();

      var revenues = viewModel.Where(v => IsRevenueAccount(v.AccountName)).ToList();
      var expenses = viewModel.Where(v => IsExpenseAccount(v.AccountName)).ToList();

      var totalRevenues = revenues.Sum(r => r.CrAmt);
      var totalExpenses = expenses.Sum(e => e.DrAmt);

      var incomeSheetViewModel = new IncomeSheetViewModel
      {
        Revenues = revenues,
        Expenses = expenses,
        TotalRevenues = totalRevenues,
        TotalExpenses = totalExpenses,
      };

      ViewBag.Month = model.Month;
      ViewBag.Year = model.Year;

      return View("~/Views/Finance/Report/IncomeSheet/PrintIncomeSheet.cshtml", incomeSheetViewModel);
    }

    private bool IsRevenueAccount(string accountName)
    {
      var revenueAccounts = new List<string> { "Sales", "Music Lesson Income" };
      return revenueAccounts.Contains(accountName);
    }

    private bool IsExpenseAccount(string accountName)
    {
      var expenseAccounts = new List<string>
    {
        "Cost of Goods Sold", "Depreciation Expense", "Wage Expense",
        "Rent Expense", "Interest Expense", "Supplies Expense", "Utilities Expense"
    };
      return expenseAccounts.Contains(accountName);
    }

  }
}

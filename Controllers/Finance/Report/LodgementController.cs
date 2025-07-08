using Exampler_ERP.Controllers.Setup;
using Exampler_ERP.Models;
using Exampler_ERP.Models.Temp;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Exampler_ERP.Controllers.Finance.Report
{
  public class LodgementController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
private readonly IHubContext<NotificationHub> _hubContext;

    private readonly ILogger<HRGlobalSettingController> _logger;
    public LodgementController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext, ILogger<HRGlobalSettingController> logger)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
_hubContext = hubContext;
 
      _logger = logger;
    }
    public async Task<IActionResult> Index()
    {
      // Fetching the list dynamically
      ViewBag.HeadofAccount_FiveList = await _utils.GetHeadofAccount_Five();

      // Returning the Lodgement view dynamically with the appropriate model
      return View("~/Views/Finance/Report/Lodgement/Lodgement.cshtml", new FI_Voucher());
    }
    [HttpPost]
    public async Task<IActionResult> Print([FromBody] LodgementReportModel model)
    {
      if (model != null)
      {
        if (model == null || model.HeadofAccount_ID == null || model.FromDate == null || model.ToDate == null)
        {
          return BadRequest("Invalid data received.");
        }

        model.FromDate = model.FromDate.Date; // Start of the day, 12:00:01 AM
        model.ToDate = model.ToDate.Date.AddDays(1).AddSeconds(-1); // End of the day, 11:59:59 PM

        // Fetch vouchers based on the filter criteria
        var VouchersQuery = _appDBContext.FI_Vouchers
            .Include(v => v.VoucherDetails)
                .ThenInclude(d => d.HeadofAccount_Five)
            .Include(v => v.VoucherType)
            .Where(v => v.VoucherDetails.Any(d =>
                d.HeadofAccount_FiveID == model.HeadofAccount_ID &&
                v.VoucherDate > model.FromDate &&
                v.VoucherDate < model.ToDate)); // Filtering criteria

        var Vouchers = await VouchersQuery.ToListAsync();

        var openingBalance = await _appDBContext.Settings_HeadofAccount_Fives
          .Where(h => h.HeadofAccount_FiveID == model.HeadofAccount_ID)
          .Select(h => h.OpeningBalance)
          .FirstOrDefaultAsync();

        var staticDate = new DateTime(2024, 12, 15);

        var GetPreviousSum = await _appDBContext.FI_VoucherDetails
            .Include(h => h.Voucher)
            .Where(h => h.HeadofAccount_FiveID == model.HeadofAccount_ID
                && h.Voucher.VoucherDate > staticDate
                && h.Voucher.VoucherDate < model.FromDate)
            .Select(h => new { h.CrAmt, h.DrAmt })
            .ToListAsync();

        decimal totalCrAmtSum = GetPreviousSum.Sum(x => (decimal)(x.CrAmt ?? 0));
        decimal totalDrAmtSum = GetPreviousSum.Sum(x => (decimal)(x.DrAmt ?? 0));
        decimal GrandTotal = totalDrAmtSum - totalCrAmtSum;
        decimal CurrentOpeningbalance = (decimal)openingBalance - GrandTotal;

        ViewBag.OpeningBalance = CurrentOpeningbalance;
        ViewBag.HeadofAccountID = model.HeadofAccount_ID;
        ViewBag.HeadofAccountName = await _appDBContext.Settings_HeadofAccount_Fives
          .Where(h => h.HeadofAccount_FiveID == model.HeadofAccount_ID)
          .Select(h => h.HeadofAccount_FiveName)
          .FirstOrDefaultAsync();
        ViewBag.FromDate = model.FromDate;
        ViewBag.ToDate = model.ToDate;

        return View("~/Views/Finance/Report/Lodgement/PrintLodgement.cshtml", Vouchers);
      }

      return BadRequest("Invalid data received.");
    }
    [HttpGet]
    public async Task<IActionResult> GetOpeningBalance(int headofAccountId)
    {
      // Replace this with your actual logic to fetch the opening balance
      var openingBalance = await _appDBContext.Settings_HeadofAccount_Fives
          .Where(h => h.HeadofAccount_FiveID == headofAccountId)
          .Select(h => h.OpeningBalance)
          .FirstOrDefaultAsync();

      return Json(new { success = true, balance = openingBalance });
    }


  }
}

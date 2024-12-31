using Exampler_ERP.Controllers.Setup;
using Exampler_ERP.Models;
using Exampler_ERP.Models.Temp;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Exampler_ERP.Controllers.Finance.Report
{
  public class LodgementController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    private readonly ILogger<HRGlobalSettingController> _logger;
    public LodgementController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, ILogger<HRGlobalSettingController> logger)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
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

        ViewBag.OpeningBalance = model.OpeningBalance ?? 0;

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

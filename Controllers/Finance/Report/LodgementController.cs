using Exampler_ERP.Controllers.Setup;
using Exampler_ERP.Models;
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
      ViewBag.HeadofAccount_FiveList = await _utils.GetHeadofAccount_FiveOnlyCashandBank();

      // Returning the Lodgement view dynamically with the appropriate model
      return View("~/Views/Finance/Report/Lodgement/Lodgement.cshtml", new FI_Voucher());
    }

  }
}

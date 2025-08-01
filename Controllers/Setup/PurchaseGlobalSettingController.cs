using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Localization;

namespace Exampler_ERP.Controllers.Setup
{
  public class PurchaseGlobalSettingController : PositionController
  {
    private readonly AppDBContext _appDBContext;
    private readonly IStringLocalizer<PurchaseGlobalSettingController> _localizer;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    private readonly IHubContext<NotificationHub> _hubContext;

    private readonly ILogger<PurchaseGlobalSettingController> _logger;

    public PurchaseGlobalSettingController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext, IStringLocalizer<PurchaseGlobalSettingController> localizer, ILogger<PurchaseGlobalSettingController> logger)
    : base(appDBContext)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
      _hubContext = hubContext;
      _localizer = localizer;

      _logger = logger;
    }
    public async Task<IActionResult> Index()
    {

      var globalSettings = await _appDBContext.HR_GlobalSettings.FirstOrDefaultAsync();

      if (globalSettings == null)
      {
        _logger.LogWarning("No global settings found.");
        return View("~/Views/Setup/PurchaseGlobalSetting/PurchaseGlobalSetting.cshtml", new HR_GlobalSetting());
      }

      return View("~/Views/Setup/PurchaseGlobalSetting/PurchaseGlobalSetting.cshtml", globalSettings);

    }
  }
}

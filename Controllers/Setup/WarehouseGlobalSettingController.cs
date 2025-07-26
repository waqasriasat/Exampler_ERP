using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Localization;

namespace Exampler_ERP.Controllers.Setup
{
  public class StoreManagementGlobalSettingController : PositionController
  {
    private readonly AppDBContext _appDBContext;
    private readonly IStringLocalizer<StoreManagementGlobalSettingController> _localizer;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    private readonly IHubContext<NotificationHub> _hubContext;

    private readonly ILogger<StoreManagementGlobalSettingController> _logger;

    public StoreManagementGlobalSettingController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext, IStringLocalizer<StoreManagementGlobalSettingController> localizer, ILogger<StoreManagementGlobalSettingController> logger)
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
        return View("~/Views/Setup/StoreManagementGlobalSetting/StoreManagementGlobalSetting.cshtml", new HR_GlobalSetting());
      }

      return View("~/Views/Setup/StoreManagementGlobalSetting/StoreManagementGlobalSetting.cshtml", globalSettings);

    }
  }
}

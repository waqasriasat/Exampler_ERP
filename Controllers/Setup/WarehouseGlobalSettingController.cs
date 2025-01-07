using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Exampler_ERP.Controllers.Setup
{
  public class WarehouseGlobalSettingController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    private readonly ILogger<WarehouseGlobalSettingController> _logger;

    public WarehouseGlobalSettingController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, ILogger<WarehouseGlobalSettingController> logger)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
      _logger = logger;
    }
    public async Task<IActionResult> Index()
    {

      var globalSettings = await _appDBContext.HR_GlobalSettings.FirstOrDefaultAsync();

      if (globalSettings == null)
      {
        _logger.LogWarning("No global settings found.");
        return View("~/Views/Setup/WarehouseGlobalSetting/WarehouseGlobalSetting.cshtml", new HR_GlobalSetting());
      }

      return View("~/Views/Setup/WarehouseGlobalSetting/WarehouseGlobalSetting.cshtml", globalSettings);

    }
  }
}

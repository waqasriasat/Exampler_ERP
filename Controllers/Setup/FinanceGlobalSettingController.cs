using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Exampler_ERP.Controllers.Setup
{
  public class FinanceGlobalSettingController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    private readonly ILogger<FinanceGlobalSettingController> _logger;

    public FinanceGlobalSettingController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, ILogger<FinanceGlobalSettingController> logger)
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
        return View("~/Views/Setup/FinanceGlobalSetting/FinanceGlobalSetting.cshtml", new HR_GlobalSetting());
      }

      return View("~/Views/Setup/FinanceGlobalSetting/FinanceGlobalSetting.cshtml", globalSettings);

    }
  }
}

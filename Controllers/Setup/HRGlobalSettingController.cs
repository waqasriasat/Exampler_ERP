using Exampler_ERP.Controllers.HR.Employeement;
using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Data.SqlTypes;

namespace Exampler_ERP.Controllers.Setup
{
  public class HRGlobalSettingController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
private readonly IHubContext<NotificationHub> _hubContext;

    private readonly ILogger<HRGlobalSettingController> _logger;

    public HRGlobalSettingController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext, ILogger<HRGlobalSettingController> logger)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
_hubContext = hubContext;
 
      _logger = logger; 
    }
    public async Task<IActionResult> Index()
    {

      var globalSettings = await _appDBContext.HR_GlobalSettings.FirstOrDefaultAsync();
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      ViewBag.DeductionTypesList = await _utils.GetDeductionTypes();
      ViewBag.OvertimeTypeList = await _utils.GetOverTimeTypes();
      return View("~/Views/Setup/HRGlobalSetting/HRGlobalSetting.cshtml", globalSettings);

    }

  }
}

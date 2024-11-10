using Exampler_ERP.Controllers.HR.Employeement;
using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlTypes;

namespace Exampler_ERP.Controllers.Setup
{
  public class HRGlobalSettingController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    private readonly ILogger<HRGlobalSettingController> _logger;

    public HRGlobalSettingController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, ILogger<HRGlobalSettingController> logger)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
      _logger = logger; 
    }
    public async Task<IActionResult> Index()
    {
    
        var globalSettings = await _appDBContext.HR_GlobalSettings.FirstOrDefaultAsync();
        ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      ViewBag.DeductionTypesList = await _utils.GetDeductionTypes();
      return View("~/Views/Setup/HRGlobalSetting/HRGlobalSetting.cshtml", globalSettings);
     
    }

  }
}

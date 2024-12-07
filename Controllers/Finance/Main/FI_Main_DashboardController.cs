using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Exampler_ERP.Controllers.Finance.Main
{
  public class FI_Main_DashboardController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;


    public FI_Main_DashboardController(AppDBContext appDBContext, IConfiguration configuration, Utils utils)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
    }
    //public async Task<IActionResult> Index()
    public async Task<IActionResult> Index()
    {
      ViewBag.CashAgainstSaleCount = await _appDBContext.Settings_CashAgainstSales.CountAsync();
      ViewBag.VoucharTypeCount = await _appDBContext.Settings_VoucharTypes.CountAsync();
      ViewBag.CategoryTypeCount = await _appDBContext.Settings_HeadofAccount_CategoryTypes.CountAsync();
      ViewBag.HeadofAccount_FirstCount = await _appDBContext.Settings_HeadofAccount_Firsts.CountAsync();
      ViewBag.HeadofAccount_SecondCount = await _appDBContext.Settings_HeadofAccount_Seconds.CountAsync();
      ViewBag.HeadofAccount_ThirdCount = await _appDBContext.Settings_HeadofAccount_Thirds.CountAsync();
      ViewBag.HeadofAccount_FourCount = await _appDBContext.Settings_HeadofAccount_Fours.CountAsync();
      ViewBag.HeadofAccount_FiveCount = await _appDBContext.Settings_HeadofAccount_Fives.CountAsync();
      return View("~/Views/Finance/Main/Dashboard/FI_Main_Dashboard.cshtml");
    }
  }
}

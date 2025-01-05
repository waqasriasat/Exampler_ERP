using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Exampler_ERP.Controllers.Finance.Main
{
  public class FI_Report_DashboardController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;


    public FI_Report_DashboardController(AppDBContext appDBContext, IConfiguration configuration, Utils utils)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
    }
    //public async Task<IActionResult> Index()
    public async Task<IActionResult> Index()
    {
      ViewBag.Lodgement = 0;
      ViewBag.TrialBalance = 0;
      ViewBag.IncomeSheet = 0;
      return View("~/Views/Finance/Main/Dashboard/FI_Report_Dashboard.cshtml");
    }
  }
}

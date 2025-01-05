using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Exampler_ERP.Controllers.Finance.Main
{
  public class FI_Management_DashboardController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;


    public FI_Management_DashboardController(AppDBContext appDBContext, IConfiguration configuration, Utils utils)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
    }
    //public async Task<IActionResult> Index()
    public async Task<IActionResult> Index()
    {
      ViewBag.Bank = await _appDBContext.FI_Banks.CountAsync();
      ViewBag.ChequeBookCount = await _appDBContext.FI_ChequeBooks.CountAsync();
      ViewBag.VendorCount = await _appDBContext.FI_Vendors.CountAsync();
      return View("~/Views/Finance/Main/Dashboard/FI_Management_Dashboard.cshtml");
    }
  }
}

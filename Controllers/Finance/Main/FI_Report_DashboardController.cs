using Microsoft.AspNetCore.Mvc;

namespace Exampler_ERP.Controllers.Finance.Main
{
  public class FI_Report_DashboardController : Controller
  {
    public IActionResult Index()
    {
      return View("~/Views/Finance/Main/Dashboard/FI_Report_Dashboard.cshtml");
    }
  }
}

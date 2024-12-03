using Microsoft.AspNetCore.Mvc;

namespace Exampler_ERP.Controllers.HR.Main
{
  public class HR_Report_DashboardController : Controller
  {
    public IActionResult Index()
    {
      return View("~/Views/HR/Main/Dashboard/HR_Report_Dashboard.cshtml");
    }
  }
}

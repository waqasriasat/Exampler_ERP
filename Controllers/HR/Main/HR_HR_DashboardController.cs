using Microsoft.AspNetCore.Mvc;

namespace Exampler_ERP.Controllers.HR.Main
{
  public class HR_HR_DashboardController : Controller
  {
    public IActionResult Index()
    {
      return View("~/Views/HR/Main/Dashboard/HR_HR_Dashboard.cshtml");
    }
  }
}

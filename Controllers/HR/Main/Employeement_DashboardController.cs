using Microsoft.AspNetCore.Mvc;

namespace Exampler_ERP.Controllers.HR.Main
{
  public class Employeement_DashboardController : Controller
  {
    public IActionResult Index()
    {
      return View("~/Views/HR/Main/Dashboard/Employeement_Dashboard.cshtml");
    }
  }
}

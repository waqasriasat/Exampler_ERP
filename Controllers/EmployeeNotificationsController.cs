using Microsoft.AspNetCore.Mvc;

namespace Exampler_ERP.Controllers
{
  public class EmployeeNotificationsController : Controller
  {
    public IActionResult Index()
    {
      return View();
    }
  }
}

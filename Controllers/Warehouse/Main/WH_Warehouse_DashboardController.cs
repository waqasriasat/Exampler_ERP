using Microsoft.AspNetCore.Mvc;

namespace Exampler_ERP.Controllers.Warehouse.Main
{
  public class WH_Warehouse_DashboardController : Controller
  {
    public IActionResult Index()
    {
      return View();
    }
  }
}

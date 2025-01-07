using Microsoft.AspNetCore.Mvc;

namespace Exampler_ERP.Controllers.Warehouse.Warehouse
{
  public class ItemController : Controller
  {
    public IActionResult Index()
    {
      return View();
    }
  }
}

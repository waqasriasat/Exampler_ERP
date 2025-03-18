using Microsoft.AspNetCore.Mvc;

namespace Exampler_ERP.Controllers.Purchase.Management
{
  public class PurchaseOrderController : Controller
  {
    public IActionResult Index()
    {
      return View();
    }
  }
}

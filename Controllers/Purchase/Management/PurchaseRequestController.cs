using Microsoft.AspNetCore.Mvc;

namespace Exampler_ERP.Controllers.Purchase.Management
{
  public class PurchaseRequestController : Controller
  {
    public IActionResult Index()
    {
      return View();
    }
  }
}

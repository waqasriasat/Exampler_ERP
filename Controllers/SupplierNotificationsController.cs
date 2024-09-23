using Exampler_ERP.Hubs;
using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Exampler_ERP.Controllers
{
  public class SupplierNotificationsController : Controller
  {
    public IActionResult Index()
    {
      return View();
    }
  }
}

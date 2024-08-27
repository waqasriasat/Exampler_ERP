using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AspnetCoreMvcFull.Models;

namespace AspnetCoreMvcFull.Controllers;

public class DashboardsController : Controller
{
  //public IActionResult Index() => View();
  public IActionResult Index()
  {
    // Check if UserID session variable is set
    if (HttpContext.Session.GetInt32("UserID") != null)
    {
      // Set ViewBag or any model data you need for the view
      ViewBag.MySession = HttpContext.Session.GetInt32("UserID").ToString();
      return View();
    }
    else
    {
      // Redirect to the Login action in AuthController if session is not set
      return RedirectToAction("Login", "Auth");
    }
  }
}

﻿using Microsoft.AspNetCore.Mvc;

namespace Exampler_ERP.Controllers.Finance.Main
{
  public class FI_Management_DashboardController : Controller
  {
    public IActionResult Index()
    {
      return View();
    }
  }
}

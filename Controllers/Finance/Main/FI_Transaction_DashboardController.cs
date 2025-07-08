using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Exampler_ERP.Controllers.Finance.Main
{
  public class FI_Transaction_DashboardController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
private readonly IHubContext<NotificationHub> _hubContext;



    public FI_Transaction_DashboardController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
_hubContext = hubContext;
 
    }
    //public async Task<IActionResult> Index()
    public async Task<IActionResult> Index()
    {
      ViewBag.JournalVoucherCount = await _appDBContext.FI_Vouchers
          .Include(v => v.VoucherType)
          .Where(v => v.VoucherType.VoucherNature == "Journal").CountAsync();

      ViewBag.TransferVoucherCount = await _appDBContext.FI_Vouchers
          .Include(v => v.VoucherType)
          .Where(v => v.VoucherType.VoucherNature == "Transfer").CountAsync();

      ViewBag.PaymentVoucherCount = await _appDBContext.FI_Vouchers
          .Include(v => v.VoucherType)
          .Where(v => v.VoucherType.VoucherNature == "Payment").CountAsync();

      ViewBag.ReceivedVoucherCount = await _appDBContext.FI_Vouchers
          .Include(v => v.VoucherType)
          .Where(v => v.VoucherType.VoucherNature == "Received").CountAsync();


      return View("~/Views/Finance/Main/Dashboard/FI_Transaction_Dashboard.cshtml");
    }
  }
}

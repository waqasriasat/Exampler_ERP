using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Exampler_ERP.Controllers.Finance.Transaction
{
  public class JournalVoucharController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;

    public JournalVoucharController(AppDBContext appDBContext, IConfiguration configuration, Utils utils)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
    }
    public async Task<IActionResult> Index(string searchHeadofAccount_FiveName)
    {
      var VoucharsQuery = _appDBContext.FI_VoucharDetails
        .Include(b => b.Vouchar)
          .Where(b => b.Vouchar.VoucharTypeID == 1 || b.Vouchar.VoucharTypeID == 6);

      if (!string.IsNullOrEmpty(searchHeadofAccount_FiveName))
      {
        VoucharsQuery = VoucharsQuery.Where(b => b.HeadofAccount_Five.HeadofAccount_FiveName.Contains(searchHeadofAccount_FiveName));
      }

      var Vouchars = await VoucharsQuery.ToListAsync();

      if (!string.IsNullOrEmpty(searchHeadofAccount_FiveName) && Vouchars.Count == 0)
      {
        TempData["ErrorMessage"] = "No Vouchar found with the name '" + searchHeadofAccount_FiveName + "'. Please check the name and try again.";
      }
      return View("~/Views/Finance/Transaction/JournalVouchar/JournalVouchar.cshtml", Vouchars);
    }
    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.VoucharTypeList = await _utils.GetVoucharType_Journal();
      ViewBag.TransactionTypeList = await _utils.GetTransactionType();
      ViewBag.HeadofAccount_FiveList = await _utils.GetHeadofAccount_Five();
      return PartialView("~/Views/Finance/Transaction/JournalVouchar/AddJournalVouchar.cshtml", new FI_Vouchar());
    }
    [HttpGet]
    public async Task<JsonResult> GetHeadofAccounts(string query)
    {
      var accounts = await _appDBContext.Settings_HeadofAccount_Fives
          .Where(a => a.HeadofAccount_FiveName.Contains(query))
          .Select(a => new { id = a.HeadofAccount_FiveID, text = a.HeadofAccount_FiveName })
          .ToListAsync();

      return Json(accounts);
    }

  }
}

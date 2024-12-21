using Exampler_ERP.Models;
using Exampler_ERP.Models.Temp;
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

      FI_Vouchar Vouchars = new FI_Vouchar();
      Vouchars.VoucharDetails.Add(new FI_VoucharDetail() { VoucharID = 1 });
      // Initialize model with one row
      var model = new JournalVoucharIndexViewModel
      {
        Vouchars = Vouchars
      };

      return PartialView("~/Views/Finance/Transaction/JournalVouchar/AddJournalVouchar.cshtml", model);
    }
    [HttpPost]
    public async Task<IActionResult> Create(JournalVoucharIndexViewModel model)
    {
      if (!ModelState.IsValid)
      {
        ViewBag.VoucharTypeList = await _utils.GetVoucharType_Journal();
        ViewBag.TransactionTypeList = await _utils.GetTransactionType();
        ViewBag.HeadofAccount_FiveList = await _utils.GetHeadofAccount_Five();

        if (model.Vouchars.VoucharDetails == null || !model.Vouchars.VoucharDetails.Any())
        {
          model.Vouchars.VoucharDetails = new List<FI_VoucharDetail> { new FI_VoucharDetail() };
        }

        if (model.Vouchars == null)
        {
          model.Vouchars = new FI_Vouchar();  // Assuming this is the correct type
        }

        return PartialView("~/Views/Finance/Transaction/JournalVouchar/AddJournalVouchar.cshtml", model);
      }

      try
      {
        model.Vouchars.VoucharDate = DateTime.Now;
        _appDBContext.FI_Vouchars.Add(model.Vouchars);

        foreach (var detail in model.Vouchars.VoucharDetails)
        {
          detail.VoucharID = model.Vouchars.VoucharID;
          _appDBContext.FI_VoucharDetails.Add(detail);
        }

        await _appDBContext.SaveChangesAsync();
        return Json(new { success = true, message = "Journal Vouchar added successfully!" });
      }
      catch (Exception ex)
      {
        return Json(new { success = false, message = "Error: " + ex.Message });
      }
    }

    [HttpGet]
    public async Task<IActionResult> GetHeadofAccounts(string searchTerm)
    {
      if (string.IsNullOrEmpty(searchTerm))
      {
        return Json(new List<object>()); // Return empty result
      }

      var accounts = await _appDBContext.Settings_HeadofAccount_Fives
          .Where(a => a.HeadofAccount_FiveName.Contains(searchTerm)) // Filter by search term
          .Select(a => new
          {
            id = a.HeadofAccount_FiveID,
            text = a.HeadofAccount_FiveName
          })
          .ToListAsync();

      return Json(accounts); // Return as JSON
    }


  }
}

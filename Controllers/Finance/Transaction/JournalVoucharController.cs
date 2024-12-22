using Exampler_ERP.Models;
using Exampler_ERP.Models.Temp;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Exampler_ERP.Controllers.Finance.Transaction
{
  public class JournalVoucherController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;

    public JournalVoucherController(AppDBContext appDBContext, IConfiguration configuration, Utils utils)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
    }
    public async Task<IActionResult> Index(string searchHeadofAccount_FiveName)
    {
      var VouchersQuery = _appDBContext.FI_Vouchers
          .Include(v => v.VoucherDetails)
          .ThenInclude(d => d.HeadofAccount_Five) // Include the nested HeadofAccount_Five
          .Where(v => v.VoucherTypeID == 3 || v.VoucherTypeID == 6);

      if (!string.IsNullOrEmpty(searchHeadofAccount_FiveName))
      {
        VouchersQuery = VouchersQuery
            .Where(v => v.VoucherDetails
                .Any(d => d.HeadofAccount_Five.HeadofAccount_FiveName
                    .Contains(searchHeadofAccount_FiveName)));
      }

      var Vouchers = await VouchersQuery.ToListAsync();

      if (!string.IsNullOrEmpty(searchHeadofAccount_FiveName) && Vouchers.Count == 0)
      {
        TempData["ErrorMessage"] = "No Voucher found with the name '" + searchHeadofAccount_FiveName + "'. Please check the name and try again.";
      }

      return View("~/Views/Finance/Transaction/JournalVoucher/JournalVoucher.cshtml", Vouchers);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
      var Vouchers = await _appDBContext.FI_Vouchers
          .Include(v => v.VoucherDetails)
          .FirstOrDefaultAsync(v => v.VoucherID == id);

      if (Vouchers == null)
      {
        return NotFound();
      }

      Vouchers.VoucherDetails.Add(new FI_VoucherDetail() { VoucherID = Vouchers.VoucherID });

      var model = new JournalVoucherIndexViewModel
      {
        Vouchers = Vouchers
      };

      ViewBag.VoucherTypeList = await _utils.GetVoucherType_Journal();
      ViewBag.TransactionTypeList = await _utils.GetTransactionType();
      ViewBag.HeadofAccount_FiveList = await _utils.GetHeadofAccount_Five();

      return PartialView("~/Views/Finance/Transaction/JournalVoucher/EditJournalVoucher.cshtml", model);
    }


    [HttpPost]
    public async Task<IActionResult> Edit(JournalVoucherIndexViewModel Voucher)
    {
      if (ModelState.IsValid)
      {
        var totalDebit = Voucher.Vouchers.VoucherDetails.Sum(v => v.DrAmt ?? 0);
        var totalCredit = Voucher.Vouchers.VoucherDetails.Sum(v => v.CrAmt ?? 0);

        if (totalDebit != totalCredit)
        {
          ModelState.AddModelError("TotalMismatch", "Total Debit and Credit amounts must be equal.");

          ViewBag.VoucherTypeList = await _utils.GetVoucherType_Journal();
          ViewBag.TransactionTypeList = await _utils.GetTransactionType();
          ViewBag.HeadofAccount_FiveList = await _utils.GetHeadofAccount_Five();

          return PartialView("~/Views/Finance/Transaction/JournalVoucher/EditJournalVoucher.cshtml", Voucher);
        }

        var existingVoucher = await _appDBContext.FI_Vouchers
            .Include(v => v.VoucherDetails)
            .FirstOrDefaultAsync(v => v.VoucherID == Voucher.Vouchers.VoucherID);

        if (existingVoucher != null)
        {
          existingVoucher.PONo = Voucher.Vouchers.PONo;
          existingVoucher.GRNNo = Voucher.Vouchers.GRNNo;
          existingVoucher.InvoiceNo = Voucher.Vouchers.InvoiceNo;
          existingVoucher.DCNo = Voucher.Vouchers.DCNo;
          existingVoucher.Description = Voucher.Vouchers.Description;

          _appDBContext.Update(existingVoucher);
          await _appDBContext.SaveChangesAsync();

          var VoucherDetailsToRemove = _appDBContext.FI_VoucherDetails
          .Where(v => v.VoucherID == Voucher.Vouchers.VoucherID)
          .ToList();

          _appDBContext.FI_VoucherDetails.RemoveRange(VoucherDetailsToRemove);

          await _appDBContext.SaveChangesAsync();
          Voucher.Vouchers.VoucherDetails.RemoveAll(e => e.HeadofAccount_FiveID == null || e.HeadofAccount_FiveID == 0);

          foreach (var detail in Voucher.Vouchers.VoucherDetails)
          {
            detail.VoucherID = Voucher.Vouchers.VoucherID;
            _appDBContext.FI_VoucherDetails.Add(detail);
          }

          await _appDBContext.SaveChangesAsync();

          return Json(new { success = true, message = "Journal Voucher Edited successfully!" });
        }
        else
        {
          return NotFound();
        }
      }

      ViewBag.VoucherTypeList = await _utils.GetVoucherType_Journal();
      ViewBag.TransactionTypeList = await _utils.GetTransactionType();
      ViewBag.HeadofAccount_FiveList = await _utils.GetHeadofAccount_Five();

      return PartialView("~/Views/Finance/Transaction/JournalVoucher/EditJournalVoucher.cshtml", Voucher);
    }



    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.VoucherTypeList = await _utils.GetVoucherType_Journal();
      ViewBag.TransactionTypeList = await _utils.GetTransactionType();
      ViewBag.HeadofAccount_FiveList = await _utils.GetHeadofAccount_Five();

      FI_Voucher Vouchers = new FI_Voucher();
      Vouchers.VoucherDetails.Add(new FI_VoucherDetail() { VoucherID = 1 });
      var model = new JournalVoucherIndexViewModel
      {
        Vouchers = Vouchers
      };

      return PartialView("~/Views/Finance/Transaction/JournalVoucher/AddJournalVoucher.cshtml", model);
    }
    [HttpPost]
    public async Task<IActionResult> Create(JournalVoucherIndexViewModel model)
    {
      if (ModelState.IsValid)
      {
        try
        {
          var VoucherType = await _appDBContext.Settings_VoucherTypes
          .FirstOrDefaultAsync(v => v.VoucherTypeID == model.Vouchers.VoucherTypeID);

          int newVoucherNumber = (VoucherType.VoucherNumber ?? 0) + 1;

          model.Vouchers.VoucherNo = VoucherType.VoucherPrefix + "-" + newVoucherNumber;
          VoucherType.VoucherNumber = newVoucherNumber;
          _appDBContext.Settings_VoucherTypes.Update(VoucherType);


          model.Vouchers.VoucherDate = DateTime.Now;
          _appDBContext.FI_Vouchers.Add(model.Vouchers);


          model.Vouchers.VoucherDetails.RemoveAll(e => e.HeadofAccount_FiveID == 0);
          foreach (var detail in model.Vouchers.VoucherDetails)
          {
            detail.VoucherID = model.Vouchers.VoucherID;
            _appDBContext.FI_VoucherDetails.Add(detail);
          }

          await _appDBContext.SaveChangesAsync();
          return Json(new { success = true, message = "Journal Voucher added successfully!" });
        }
        catch (Exception ex)
        {
          return Json(new { success = false, message = "Error: " + ex.Message });
        }
        
      }

      ViewBag.VoucherTypeList = await _utils.GetVoucherType_Journal();
      ViewBag.TransactionTypeList = await _utils.GetTransactionType();
      ViewBag.HeadofAccount_FiveList = await _utils.GetHeadofAccount_Five();

      if (model.Vouchers.VoucherDetails == null || !model.Vouchers.VoucherDetails.Any())
      {
        model.Vouchers.VoucherDetails = new List<FI_VoucherDetail> { new FI_VoucherDetail() };
      }

      if (model.Vouchers == null)
      {
        model.Vouchers = new FI_Voucher(); 
      }

      return PartialView("~/Views/Finance/Transaction/JournalVoucher/AddJournalVoucher.cshtml", model);
    }

    [HttpGet]
    public async Task<IActionResult> GetHeadofAccounts(string searchTerm)
    {
      if (string.IsNullOrEmpty(searchTerm))
      {
        return Json(new List<object>()); // Return empty result
      }

      var accounts = await _appDBContext.Settings_HeadofAccount_Fives
          .Where(a => a.HeadofAccount_FiveName.Contains(searchTerm)) 
          .Select(a => new
          {
            id = a.HeadofAccount_FiveID,
            text = a.HeadofAccount_FiveName
          })
          .ToListAsync();

      return Json(accounts); 
    }


  }
}

using Exampler_ERP.Hubs;
using Exampler_ERP.Models;
using Exampler_ERP.Models.Temp;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;
using OfficeOpenXml;
using Microsoft.Extensions.Localization;

namespace Exampler_ERP.Controllers.Finance.Transaction
{
  public class PaymentVoucherController : PositionController
  {
    private readonly AppDBContext _appDBContext;
    private readonly IStringLocalizer<PaymentVoucherController> _localizer;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    private readonly IHubContext<NotificationHub> _hubContext;


    public PaymentVoucherController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext, IStringLocalizer<PaymentVoucherController> localizer) 
    : base(appDBContext)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
      _hubContext = hubContext;
      _localizer = localizer;

    }
    public async Task<IActionResult> Index(string searchHeadofAccount_FiveName)
    {
      var VouchersQuery = _appDBContext.FI_Vouchers
          .Include(v => v.VoucherDetails)
          .ThenInclude(d => d.HeadofAccount_Five)
          .Include(v => v.VoucherType)
          .Where(v => v.VoucherType.VoucherNature == "Payment");

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
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "No Voucher found with the name '" + searchHeadofAccount_FiveName + "'. Please check the name and try again.");
      }

      return View("~/Views/Finance/Transaction/PaymentVoucher/PaymentVoucher.cshtml", Vouchers);
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

      var model = new PaymentVoucherIndexViewModel
      {
        Vouchers = Vouchers
      };

      ViewBag.VoucherTypeList = await _utils.GetVoucherType_Payment();
      ViewBag.TransactionTypeList = await _utils.GetTransactionType();
      ViewBag.HeadofAccount_DebitList = await _utils.GetHeadofAccount_FiveOnlyCashandBank();
      ViewBag.HeadofAccount_CreditList = await _utils.GetHeadofAccount_FiveOnlyPayable();
      ViewBag.IntrumentTypeList = await _utils.GetIntrumentType();
      ViewBag.HeadofAccount_FiveList = await _utils.GetHeadofAccount_Five();
      ViewBag.HeadofAccount_IncomeTaxPayableList = await _utils.GetHeadofAccount_FiveIncomeTaxPayable();
      ViewBag.HeadofAccount_SaleTaxPayableList = await _utils.GetHeadofAccount_FiveSaleTaxPayable();

      return PartialView("~/Views/Finance/Transaction/PaymentVoucher/EditPaymentVoucher.cshtml", model);
    }


    [HttpPost]
    public async Task<IActionResult> Edit(PaymentVoucherIndexViewModel Voucher)
    {
      if (ModelState.IsValid)
      {
        var totalDebit = Voucher.Vouchers.VoucherDetails.Sum(v => v.DrAmt ?? 0);
        var totalCredit = Voucher.Vouchers.VoucherDetails.Sum(v => v.CrAmt ?? 0);

        if (totalDebit != totalCredit)
        {
          ModelState.AddModelError("TotalMismatch", "Total Debit and Credit amounts must be equal.");

          ViewBag.VoucherTypeList = await _utils.GetVoucherType_Payment();
          ViewBag.TransactionTypeList = await _utils.GetTransactionType();
          ViewBag.HeadofAccount_DebitList = await _utils.GetHeadofAccount_FiveOnlyCashandBank();
          ViewBag.HeadofAccount_CreditList = await _utils.GetHeadofAccount_FiveOnlyPayable();
          ViewBag.IntrumentTypeList = await _utils.GetIntrumentType();
          ViewBag.HeadofAccount_FiveList = await _utils.GetHeadofAccount_Five();
          ViewBag.HeadofAccount_IncomeTaxPayableList = await _utils.GetHeadofAccount_FiveIncomeTaxPayable();
          ViewBag.HeadofAccount_SaleTaxPayableList = await _utils.GetHeadofAccount_FiveSaleTaxPayable();

          return PartialView("~/Views/Finance/Transaction/PaymentVoucher/EditPaymentVoucher.cshtml", Voucher);
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
          existingVoucher.InstruDate = Voucher.Vouchers.InstruDate;
          existingVoucher.InstruDetail = Voucher.Vouchers.InstruDetail;
          existingVoucher.InstruNumber = Voucher.Vouchers.InstruNumber;
          existingVoucher.InstruType = Voucher.Vouchers.InstruType;
          existingVoucher.PayeeName = Voucher.Vouchers.PayeeName;

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

          return Json(new { success = true, message = "Payment Voucher Edited successfully!" });
        }
        else
        {
          return NotFound();
        }
      }

      ViewBag.VoucherTypeList = await _utils.GetVoucherType_Payment();
      ViewBag.TransactionTypeList = await _utils.GetTransactionType();
      ViewBag.HeadofAccount_DebitList = await _utils.GetHeadofAccount_FiveOnlyCashandBank();
      ViewBag.HeadofAccount_CreditList = await _utils.GetHeadofAccount_FiveOnlyPayable();
      ViewBag.IntrumentTypeList = await _utils.GetIntrumentType();
      ViewBag.HeadofAccount_FiveList = await _utils.GetHeadofAccount_Five();
      ViewBag.HeadofAccount_IncomeTaxPayableList = await _utils.GetHeadofAccount_FiveIncomeTaxPayable();
      ViewBag.HeadofAccount_SaleTaxPayableList = await _utils.GetHeadofAccount_FiveSaleTaxPayable();

      return PartialView("~/Views/Finance/Transaction/PaymentVoucher/EditPaymentVoucher.cshtml", Voucher);
    }



    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.VoucherTypeList = await _utils.GetVoucherType_Payment();
      ViewBag.TransactionTypeList = await _utils.GetTransactionType();
      ViewBag.HeadofAccount_DebitList = await _utils.GetHeadofAccount_FiveOnlyCashandBank();
      ViewBag.HeadofAccount_CreditList = await _utils.GetHeadofAccount_FiveOnlyPayable();
      ViewBag.IntrumentTypeList = await _utils.GetIntrumentType();
      ViewBag.HeadofAccount_FiveList = await _utils.GetHeadofAccount_Five();
      ViewBag.HeadofAccount_IncomeTaxPayableList = await _utils.GetHeadofAccount_FiveIncomeTaxPayable();
      ViewBag.HeadofAccount_SaleTaxPayableList = await _utils.GetHeadofAccount_FiveSaleTaxPayable();

      FI_Voucher Vouchers = new FI_Voucher();
      Vouchers.VoucherDetails.Add(new FI_VoucherDetail() { VoucherID = 1 });
      var model = new PaymentVoucherIndexViewModel
      {
        Vouchers = Vouchers
      };

      return PartialView("~/Views/Finance/Transaction/PaymentVoucher/AddPaymentVoucher.cshtml", model);
    }
    [HttpPost]
    public async Task<IActionResult> Create(PaymentVoucherIndexViewModel model)
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

          model.Vouchers.FinalApprovalID = 0;
          model.Vouchers.VoucherDate = DateTime.Now;
          _appDBContext.FI_Vouchers.Add(model.Vouchers);


          model.Vouchers.VoucherDetails.RemoveAll(e => e.HeadofAccount_FiveID == 0);
          model.Vouchers.VoucherDetails.RemoveAll(e => e.HeadofAccount_FiveID == null || e.HeadofAccount_FiveID == 0);

          foreach (var detail in model.Vouchers.VoucherDetails)
          {
            detail.VoucherID = model.Vouchers.VoucherID;
            _appDBContext.FI_VoucherDetails.Add(detail);
          }

          await _appDBContext.SaveChangesAsync();

          var voucherId = model.Vouchers.VoucherID;
          if (voucherId > 0)
          {
            var processCount = await _appDBContext.CR_ProcessTypeApprovalSetups
                                .Where(pta => pta.ProcessTypeID > 0 && pta.ProcessTypeID == 18)
                                .CountAsync();

            if (processCount > 0)
            {
              var newProcessTypeApproval = new CR_ProcessTypeApproval
              {
                ProcessTypeID = 18,
                Notes = "Create New Payment Voucher",
                Date = DateTime.Now,
                EmployeeID = HttpContext.Session.GetInt32("UserID") ?? default(int),
                UserID = HttpContext.Session.GetInt32("UserID") ?? default(int),
                TransactionID = voucherId
              };

              _appDBContext.CR_ProcessTypeApprovals.Add(newProcessTypeApproval);
              await _appDBContext.SaveChangesAsync();

              var nextApprovalSetup = await _appDBContext.CR_ProcessTypeApprovalSetups
                                          .Where(pta => pta.ProcessTypeID == 18 && pta.Rank == 1)
                                          .FirstOrDefaultAsync();

              if (nextApprovalSetup != null)
              {
                var newProcessTypeApprovalDetail = new CR_ProcessTypeApprovalDetail
                {
                  ProcessTypeApprovalID = newProcessTypeApproval.ProcessTypeApprovalID,
                  Date = DateTime.Now,
                  RoleID = nextApprovalSetup.RoleTypeID,
                  AppID = 0,
                  AppUserID = 0,
                  Notes = null,
                  Rank = nextApprovalSetup.Rank
                };

                _appDBContext.CR_ProcessTypeApprovalDetails.Add(newProcessTypeApprovalDetail);
                await _appDBContext.SaveChangesAsync();
                await _hubContext.Clients.All.SendAsync("ReceiveProcessNotification");
              }
              else
              {
                return Json(new { success = false, message = "Next approval setup not found." });
              }
            }
            else
            {
              model.Vouchers.FinalApprovalID = 1;
              _appDBContext.FI_Vouchers.Update(model.Vouchers);
              await _appDBContext.SaveChangesAsync();
              await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Payment Voucher successfully. No process setup found, Payment Voucher Approved.");
              return Json(new { success = true, message = "No process setup found, Payment Voucher Approved." });
            }
          }
          await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Payment Voucher Created successfully. Continue to the Approval Process Setup for Payment Voucher Approved.");

          return Json(new { success = true });
        }
        catch (Exception ex)
        {
          return Json(new { success = false, message = "Error: " + ex.Message });
        }

      }

      ViewBag.VoucherTypeList = await _utils.GetVoucherType_Payment();
      ViewBag.TransactionTypeList = await _utils.GetTransactionType();
      ViewBag.HeadofAccount_DebitList = await _utils.GetHeadofAccount_FiveOnlyCashandBank();
      ViewBag.HeadofAccount_CreditList = await _utils.GetHeadofAccount_FiveOnlyPayable();
      ViewBag.IntrumentTypeList = await _utils.GetIntrumentType();
      ViewBag.HeadofAccount_FiveList = await _utils.GetHeadofAccount_Five();
      ViewBag.HeadofAccount_IncomeTaxPayableList = await _utils.GetHeadofAccount_FiveIncomeTaxPayable();
      ViewBag.HeadofAccount_SaleTaxPayableList = await _utils.GetHeadofAccount_FiveSaleTaxPayable();


      if (model.Vouchers.VoucherDetails == null || !model.Vouchers.VoucherDetails.Any())
      {
        model.Vouchers.VoucherDetails = new List<FI_VoucherDetail> { new FI_VoucherDetail() };
      }

      if (model.Vouchers == null)
      {
        model.Vouchers = new FI_Voucher();
      }

      return PartialView("~/Views/Finance/Transaction/PaymentVoucher/AddPaymentVoucher.cshtml", model);
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
    [HttpPost]
    public async Task<IActionResult> GetTaxation(int? headofAccount)
    {
      if (headofAccount == null || headofAccount == 0)
      {
        return Json(new List<object>());
      }

      var accounts = await _appDBContext.FI_Vendors
          .Where(a => a.HeadofAccount_FiveID == headofAccount)
          .Select(a => new
          {
            saletaxid = a.SaleTax_ID,
            saletaxvalue = a.STNRate,
            incometaxid = a.IncomTaxWithHoding_ID,
            incometaxvalue = a.NTNRate,
            payeename = a.PayeeName
          })
          .ToListAsync();

      return Json(accounts);
    }

    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      // Fetching data with related entities
      var Vouchers = await _appDBContext.FI_Vouchers
          .Include(v => v.VoucherDetails)
          .ThenInclude(d => d.HeadofAccount_Five)
          .Include(v => v.VoucherType)
          .Where(v => v.VoucherType.VoucherNature == "Payment")
          .ToListAsync();

      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add(_localizer["lbl_PaymentVoucher"]);

        // Adding header row
        worksheet.Cells["A1"].Value = _localizer["lbl_VoucherID"];
        worksheet.Cells["B1"].Value = _localizer["lbl_Date"];
        worksheet.Cells["C1"].Value = _localizer["lbl_HeadofAccount"];
        worksheet.Cells["D1"].Value = _localizer["lbl_CreditAmount"];

        worksheet.Cells["A1:D1"].Style.Font.Bold = true; // Bold header

        int row = 2; // Starting row for data

        foreach (var voucher in Vouchers)
        {
          var drDetails = voucher.VoucherDetails.FirstOrDefault(d => d.DRCR == 1);
          var crDetails = voucher.VoucherDetails.FirstOrDefault(d => d.DRCR == 2);

          worksheet.Cells[row, 1].Value = voucher.VoucherNo; // Voucher #
          worksheet.Cells[row, 2].Value = voucher.VoucherDate; // Date

          if (drDetails != null || crDetails != null)
          {
            string drHead = drDetails?.HeadofAccount_Five?.HeadofAccount_FiveName ?? "N/A";
            string crHead = crDetails?.HeadofAccount_Five?.HeadofAccount_FiveName ?? "N/A";

            worksheet.Cells[row, 3].Value = $"{drHead} -> {crHead}"; // Combined DR and CR Head of Account
          }

          if (crDetails != null)
          {
            worksheet.Cells[row, 4].Value = crDetails.CrAmt.HasValue
                ? crDetails.CrAmt.Value.ToString("N2") // Credit Amount
                : "0.00";
          }

          row++; // Move to the next row
        }

        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;

        string excelName = _localizer["lbl_PaymentVoucher"]+$"-{DateTime.Now:yyyyMMddHHmmssfff}.xlsx";
        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }

    public async Task<IActionResult> Print()
    {
      var VouchersQuery = _appDBContext.FI_Vouchers
        .Include(v => v.VoucherDetails)
        .ThenInclude(d => d.HeadofAccount_Five)
        .Include(v => v.VoucherType)
        .Where(v => v.VoucherType.VoucherNature == "Payment");

      var Vouchers = await VouchersQuery.ToListAsync();
      return View("~/Views/Finance/Transaction/PaymentVoucher/PrintPaymentVoucher.cshtml", Vouchers);
    }

  }
}

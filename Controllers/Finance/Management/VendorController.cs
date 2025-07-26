using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;
using OfficeOpenXml;
using Microsoft.Extensions.Localization;

namespace Exampler_ERP.Controllers.Finance.Management
{
  public class VendorController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IStringLocalizer<VendorController> _localizer;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    private readonly IHubContext<NotificationHub> _hubContext;


    public VendorController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext, IStringLocalizer<VendorController> localizer)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
      _hubContext = hubContext;
      _localizer = localizer;

    }
    public async Task<IActionResult> Index(string searchVendorName)
    {
      var VendorsQuery = _appDBContext.FI_Vendors
        .Include(b => b.HeadofAccount_Five)
          .Where(b => b.DeleteYNID != 1);

      if (!string.IsNullOrEmpty(searchVendorName))
      {
        VendorsQuery = VendorsQuery.Where(b => b.HeadofAccount_Five.HeadofAccount_FiveName.Contains(searchVendorName));
      }

      var Vendors = await VendorsQuery.ToListAsync();

      if (!string.IsNullOrEmpty(searchVendorName) && Vendors.Count == 0)
      {
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "No Vendor found with the name '" + searchVendorName + "'. Please check the name and try again.");
      }
      return View("~/Views/Finance/Management/Vendor/Vendor.cshtml", Vendors);
    }


    public async Task<IActionResult> Vendor()
    {
      var Vendors = await _appDBContext.FI_Vendors.ToListAsync();
      return Ok(Vendors);
    }// Add the Edit action
    public async Task<IActionResult> Edit(int id)
    {
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      ViewBag.VendorList = await _utils.GetHeadofAccount_FiveOnlyVendor();
      ViewBag.SaleTaxList = await _utils.GetHeadofAccount_FiveSaleTaxPayable();
      ViewBag.IncomeTaxList = await _utils.GetHeadofAccount_FiveIncomeTaxPayable();
      var Vendor = await _appDBContext.FI_Vendors.FindAsync(id);
      if (Vendor == null)
      {
        return NotFound();
      }
      Vendor.UserName = CR_CipherKey.Decrypt(Vendor.UserName);
      Vendor.Password = CR_CipherKey.Decrypt(Vendor.Password);
      return PartialView("~/Views/Finance/Management/Vendor/EditVendor.cshtml", Vendor);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(FI_Vendor Vendor)
    {
      if (ModelState.IsValid)
      {
        if (string.IsNullOrEmpty(Vendor.PayeeName) && string.IsNullOrEmpty(Vendor.UserName) && string.IsNullOrEmpty(Vendor.Password) && Vendor.VendorID.ToString().Length > 0)
        {
          return Json(new { success = false, message = "Payee Name, User Name, Password and Vendor Name field is required. Please enter a valid text value." });
        }
        Vendor.UserName = CR_CipherKey.Encrypt(Vendor.UserName);
        Vendor.Password = CR_CipherKey.Encrypt(Vendor.Password);
        _appDBContext.Update(Vendor);
        await _appDBContext.SaveChangesAsync();
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Vendor updated successfully.");
        return Json(new { success = true });
      }
      return Json(new { success = false, message = "Error creating Vendor. Please check the inputs." });
    }
    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      ViewBag.VendorList = await _utils.GetHeadofAccount_FiveOnlyVendor();
      ViewBag.SaleTaxList = await _utils.GetHeadofAccount_FiveSaleTaxPayable();
      ViewBag.IncomeTaxList = await _utils.GetHeadofAccount_FiveIncomeTaxPayable();

      return PartialView("~/Views/Finance/Management/Vendor/AddVendor.cshtml", new FI_Vendor());
    }

    [HttpPost]
    public async Task<IActionResult> Create(FI_Vendor Vendor)
    {
      if (ModelState.IsValid)
      {
        if (string.IsNullOrEmpty(Vendor.PayeeName) && string.IsNullOrEmpty(Vendor.UserName) && string.IsNullOrEmpty(Vendor.Password) && Vendor.VendorID.ToString().Length > 0)
        {
          return Json(new { success = false, message = "Payee Name, User Name, Password and Vendor Name field is required. Please enter a valid text value." });
        }
        Vendor.UserName = CR_CipherKey.Encrypt(Vendor.UserName);
        Vendor.Password = CR_CipherKey.Encrypt(Vendor.Password);
        Vendor.DeleteYNID = 0;

        _appDBContext.FI_Vendors.Add(Vendor);
        await _appDBContext.SaveChangesAsync();

        await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Vendor created successfully.");
        return Json(new { success = true });
      }

      return Json(new { success = false, message = "Error creating Vendor. Please check the inputs." });
    }

    public async Task<IActionResult> Delete(int id)
    {
      var Vendor = await _appDBContext.FI_Vendors.FindAsync(id);
      if (Vendor == null)
      {
        return NotFound();
      }

      Vendor.ActiveYNID = 2;
      Vendor.DeleteYNID = 1;

      _appDBContext.FI_Vendors.Update(Vendor);
      await _appDBContext.SaveChangesAsync();
      await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Vendor deleted successfully.");

      return Json(new { success = true });
    }
    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var Vendors = await _appDBContext.FI_Vendors
        .Include(b => b.HeadofAccount_Five)
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();

      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add(_localizer["lbl_Vendor"]);

        // Adding column headers
        worksheet.Cells["A1"].Value = _localizer["lbl_VendorID"];
        worksheet.Cells["B1"].Value = _localizer["lbl_VendorName"];
        worksheet.Cells["C1"].Value = _localizer["lbl_VendorPayee"];
        worksheet.Cells["D1"].Value = _localizer["lbl_Cell"];
        worksheet.Cells["E1"].Value = _localizer["lbl_Phone"]; 
        worksheet.Cells["F1"].Value = _localizer["lbl_PersonName"];
        worksheet.Cells["G1"].Value = _localizer["lbl_Fax"]; 
        worksheet.Cells["H1"].Value = _localizer["lbl_Filer"];
        worksheet.Cells["I1"].Value = _localizer["lbl_STN"];
        worksheet.Cells["J1"].Value = _localizer["lbl_STNRate"];
        worksheet.Cells["K1"].Value = _localizer["lbl_NTN"];
        worksheet.Cells["L1"].Value = _localizer["lbl_NTNRate"];
        worksheet.Cells["M1"].Value = _localizer["lbl_IncomeTax(Withholding)"];
        worksheet.Cells["N1"].Value = _localizer["lbl_SaleTaxAccount"];
        worksheet.Cells["O1"].Value = _localizer["lbl_Province"];
        worksheet.Cells["P1"].Value = _localizer["lbl_Federal"];
        worksheet.Cells["Q1"].Value = _localizer["lbl_PaymentSection"];
        worksheet.Cells["R1"].Value = _localizer["lbl_Address"];

        // Adding data rows
        for (int i = 0; i < Vendors.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = Vendors[i].VendorID;
          worksheet.Cells[i + 2, 2].Value = Vendors[i].HeadofAccount_Five.HeadofAccount_FiveName;
          worksheet.Cells[i + 2, 3].Value = Vendors[i].PayeeName;
          worksheet.Cells[i + 2, 4].Value = Vendors[i].Cell;
          worksheet.Cells[i + 2, 5].Value = Vendors[i].Phone;
          worksheet.Cells[i + 2, 6].Value = Vendors[i].PersonName;
          worksheet.Cells[i + 2, 7].Value = Vendors[i].Fax;
          worksheet.Cells[i + 2, 8].Value = Vendors[i].Filer.HasValue ? Vendors[i].Filer.ToString() : string.Empty;
          worksheet.Cells[i + 2, 9].Value = Vendors[i].STN;
          worksheet.Cells[i + 2, 10].Value = Vendors[i].STNRate.HasValue ? Vendors[i].STNRate.ToString() : string.Empty;
          worksheet.Cells[i + 2, 11].Value = Vendors[i].NTN;
          worksheet.Cells[i + 2, 12].Value = Vendors[i].NTNRate.HasValue ? Vendors[i].NTNRate.ToString() : string.Empty;
          worksheet.Cells[i + 2, 13].Value = Vendors[i].IncomTaxWithHoding_ID.HasValue ? Vendors[i].IncomTaxWithHoding_ID.ToString() : string.Empty;
          worksheet.Cells[i + 2, 14].Value = Vendors[i].SaleTax_ID.HasValue ? Vendors[i].SaleTax_ID.ToString() : string.Empty;
          worksheet.Cells[i + 2, 15].Value = Vendors[i].Province;
          worksheet.Cells[i + 2, 16].Value = Vendors[i].Federal;
          worksheet.Cells[i + 2, 17].Value = Vendors[i].PaymentSec;
          worksheet.Cells[i + 2, 18].Value = Vendors[i].Address;
        }

        // Apply formatting
        worksheet.Cells["A1:R1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();


        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = _localizer["lbl_Vendor"]+$"-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
    public async Task<IActionResult> Print()
    {
      var Vendors = await _appDBContext.FI_Vendors
        .Include(b => b.HeadofAccount_Five)
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();
      return View("~/Views/Finance/Management/Vendor/PrintVendor.cshtml", Vendors);
    }


  }
}

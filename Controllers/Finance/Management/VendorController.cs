using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace Exampler_ERP.Controllers.Finance.Management
{
  public class VendorController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;

    public VendorController(AppDBContext appDBContext, IConfiguration configuration, Utils utils)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
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
        TempData["ErrorMessage"] = "No Vendor found with the name '" + searchVendorName + "'. Please check the name and try again.";
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
      return PartialView("~/Views/Finance/Management/Vendor/EditVendor.cshtml", Vendor);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(FI_Vendor Vendor)
    {
      if (ModelState.IsValid)
      {
        //if (string.IsNullOrEmpty(Vendor.HeadofAccount_Five.HeadofAccount_FiveName))
        //{
        //  return Json(new { success = false, message = "Vendor Name field is required. Please enter a valid text value." });
        //}


        _appDBContext.Update(Vendor);
        await _appDBContext.SaveChangesAsync();
        TempData["SuccessMessage"] = "Vendor updated successfully.";
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
        //if (string.IsNullOrEmpty(Vendor.HeadofAccount_Five.HeadofAccount_FiveName))
        //{
        //  return Json(new { success = false, message = "Vendor Name field is required. Please enter a valid text value." });
        //}


        Vendor.DeleteYNID = 0;

        _appDBContext.FI_Vendors.Add(Vendor);
        await _appDBContext.SaveChangesAsync();

        TempData["SuccessMessage"] = "Vendor created successfully.";
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
      TempData["SuccessMessage"] = "Vendor deleted successfully.";

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
        var worksheet = package.Workbook.Worksheets.Add("Vendors");

        // Adding column headers
        worksheet.Cells["A1"].Value = "Vendor ID";
        worksheet.Cells["B1"].Value = "Vendor Name";
        worksheet.Cells["C1"].Value = "Payee Name";
        worksheet.Cells["D1"].Value = "Cell";
        worksheet.Cells["E1"].Value = "Phone";
        worksheet.Cells["F1"].Value = "Person Name";
        worksheet.Cells["G1"].Value = "Fax";
        worksheet.Cells["H1"].Value = "Filer";
        worksheet.Cells["I1"].Value = "STN";
        worksheet.Cells["J1"].Value = "STN Rate";
        worksheet.Cells["K1"].Value = "NTN";
        worksheet.Cells["L1"].Value = "NTN Rate";
        worksheet.Cells["M1"].Value = "Income Tax Withholding ID";
        worksheet.Cells["N1"].Value = "Sale Tax ID";
        worksheet.Cells["O1"].Value = "Province";
        worksheet.Cells["P1"].Value = "Federal";
        worksheet.Cells["Q1"].Value = "Payment Section";
        worksheet.Cells["R1"].Value = "Address";

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
        string excelName = $"Vendors-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

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

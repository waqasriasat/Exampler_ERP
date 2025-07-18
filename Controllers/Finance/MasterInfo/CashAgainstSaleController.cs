using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;
using OfficeOpenXml;
using Microsoft.Extensions.Localization;

namespace Exampler_ERP.Controllers.Finance.MasterInfo
{
  public class CashAgainstSaleController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IStringLocalizer<CashAgainstSaleController> _localizer;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    private readonly IHubContext<NotificationHub> _hubContext;


    public CashAgainstSaleController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext, IStringLocalizer<CashAgainstSaleController> localizer)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
      _hubContext = hubContext;
      _localizer = localizer;

    }
    public async Task<IActionResult> Index(string searchBranchName)
    {
      var CashAgainstSalesQuery = _appDBContext.Settings_CashAgainstSales
          .Include(b => b.BranchType)
          .Include(b => b.HeadofAccount_Five)
          .Where(b => b.DeleteYNID != 1);

      if (!string.IsNullOrEmpty(searchBranchName))
      {
        CashAgainstSalesQuery = CashAgainstSalesQuery.Where(b => b.BranchType.BranchTypeName.Contains(searchBranchName));
      }

      var CashAgainstSales = await CashAgainstSalesQuery.ToListAsync();

      if (!string.IsNullOrEmpty(searchBranchName) && CashAgainstSales.Count == 0)
      {
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "No Branch Name found with the name '" + searchBranchName + "'. Please check the name and try again.");
      }
      return View("~/Views/Finance/MasterInfo/CashAgainstSale/CashAgainstSale.cshtml", CashAgainstSales);
    }


    public async Task<IActionResult> CashAgainstSale()
    {
      var CashAgainstSales = await _appDBContext.Settings_CashAgainstSales.ToListAsync();
      return Ok(CashAgainstSales);
    }// Add the Edit action
    public async Task<IActionResult> Edit(int id)
    {
      ViewBag.BranchList = await _utils.GetBranchs();
      ViewBag.FiveList = await _utils.GetHeadofAccount_Five();
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      var CashAgainstSale = await _appDBContext.Settings_CashAgainstSales.FindAsync(id);
      if (CashAgainstSale == null)
      {
        return NotFound();
      }
      return PartialView("~/Views/Finance/MasterInfo/CashAgainstSale/EditCashAgainstSale.cshtml", CashAgainstSale);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Settings_CashAgainstSale CashAgainstSale)
    {
      if (ModelState.IsValid)
      {

        _appDBContext.Update(CashAgainstSale);
        await _appDBContext.SaveChangesAsync();
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Cash Against Sale Account connected updated successfully.");
        return Json(new { success = true });
      }
      return Json(new { success = false, message = "Error connecting Cash Against Sale Account. Please check the inputs." });
    }
    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.BranchList = await _utils.GetBranchs();
      ViewBag.FiveList = await _utils.GetHeadofAccount_Five();
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      return PartialView("~/Views/Finance/MasterInfo/CashAgainstSale/AddCashAgainstSale.cshtml", new Settings_CashAgainstSale());
    }

    [HttpPost]
    public async Task<IActionResult> Create(Settings_CashAgainstSale CashAgainstSale)
    {
      if (ModelState.IsValid)
      {

        CashAgainstSale.DeleteYNID = 0;

        _appDBContext.Settings_CashAgainstSales.Add(CashAgainstSale);
        await _appDBContext.SaveChangesAsync();

        await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Cash Against Sale Account connected successfully.");
        return Json(new { success = true });
      }

      return Json(new { success = false, message = "Error connecting Cash Against Sale Account. Please check the inputs." });
    }

    public async Task<IActionResult> Delete(int id)
    {
      var CashAgainstSale = await _appDBContext.Settings_CashAgainstSales.FindAsync(id);
      if (CashAgainstSale == null)
      {
        return NotFound();
      }

      CashAgainstSale.ActiveYNID = 2;
      CashAgainstSale.DeleteYNID = 1;

      _appDBContext.Settings_CashAgainstSales.Update(CashAgainstSale);
      await _appDBContext.SaveChangesAsync();
      await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Cash Against Sale Account deleted successfully.");

      return Json(new { success = true });
    }
    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var CashAgainstSales = await _appDBContext.Settings_CashAgainstSales
          .Include(b => b.BranchType)
          .Include(b => b.HeadofAccount_Five)
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();

      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add("CashAgainstSales");
        worksheet.Cells["A1"].Value = _localizer["lbl_BranchName"];
        worksheet.Cells["B1"].Value = "CashAgainstSale Account";
        worksheet.Cells["C1"].Value = _localizer["lbl_Active"];


        for (int i = 0; i < CashAgainstSales.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = CashAgainstSales[i].BranchType.BranchTypeName;
          worksheet.Cells[i + 2, 2].Value = CashAgainstSales[i].HeadofAccount_Five.HeadofAccount_FiveName;
          worksheet.Cells[i + 2, 3].Value = CashAgainstSales[i].ActiveYNID == 1 ? _localizer["lbl_Yes"] : _localizer["lbl_No"];
        }

        worksheet.Cells["A1:l1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = $"CashAgainstSales-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
    public async Task<IActionResult> Print()
    {
      var CashAgainstSales = await _appDBContext.Settings_CashAgainstSales
          .Include(b => b.BranchType)
          .Include(b => b.HeadofAccount_Five)
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();
      return View("~/Views/Finance/MasterInfo/CashAgainstSale/PrintCashAgainstSale.cshtml", CashAgainstSales);
    }


  }
}

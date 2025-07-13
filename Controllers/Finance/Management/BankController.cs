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
  public class BankController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IStringLocalizer<BankController> _localizer;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    private readonly IHubContext<NotificationHub> _hubContext;


    public BankController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext, IStringLocalizer<BankController> localizer)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
      _hubContext = hubContext;
      _localizer = localizer;

    }
    public async Task<IActionResult> Index(string searchBankName)
    {
      var BanksQuery = _appDBContext.FI_Banks
          .Where(b => b.DeleteYNID != 1);

      if (!string.IsNullOrEmpty(searchBankName))
      {
        BanksQuery = BanksQuery.Where(b => b.BankName.Contains(searchBankName));
      }

      var Banks = await BanksQuery.ToListAsync();

      if (!string.IsNullOrEmpty(searchBankName) && Banks.Count == 0)
      {
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "No Bank found with the name '" + searchBankName + "'. Please check the name and try again.");
      }
      return View("~/Views/Finance/Management/Bank/Bank.cshtml", Banks);
    }


    public async Task<IActionResult> Bank()
    {
      var Banks = await _appDBContext.FI_Banks.ToListAsync();
      return Ok(Banks);
    }// Add the Edit action
    public async Task<IActionResult> Edit(int id)
    {
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      var Bank = await _appDBContext.FI_Banks.FindAsync(id);
      if (Bank == null)
      {
        return NotFound();
      }
      return PartialView("~/Views/Finance/Management/Bank/EditBank.cshtml", Bank);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(FI_Bank Bank)
    {
      if (ModelState.IsValid)
      {
        if (string.IsNullOrEmpty(Bank.BankName))
        {
          return Json(new { success = false, message = "Bank Name field is required. Please enter a valid text value." });
        }


        _appDBContext.Update(Bank);
        await _appDBContext.SaveChangesAsync();
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Bank updated successfully.");
        return Json(new { success = true });
      }
      return Json(new { success = false, message = "Error creating Bank. Please check the inputs." });
    }
    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      return PartialView("~/Views/Finance/Management/Bank/AddBank.cshtml", new FI_Bank());
    }

    [HttpPost]
    public async Task<IActionResult> Create(FI_Bank Bank)
    {
      if (ModelState.IsValid)
      {
        if (string.IsNullOrEmpty(Bank.BankName))
        {
          return Json(new { success = false, message = "Bank Name field is required. Please enter a valid text value." });
        }


        Bank.DeleteYNID = 0;
        _appDBContext.FI_Banks.Add(Bank);
        await _appDBContext.SaveChangesAsync();

        var bankName = Bank.BankName;
        if (bankName != "")
        {
          var headofAccount_Five = new Settings_HeadofAccount_Five
          {
            HeadofAccount_FiveName = bankName,
            HeadofAccount_FourID = 1,
            CategoryTypeID = 3,
            GroupTypeID = 1,
            DeleteYNID = 0,
            ActiveYNID = 1,
            OpeningBalance = 0
          };

          _appDBContext.Settings_HeadofAccount_Fives.Add(headofAccount_Five);
          await _appDBContext.SaveChangesAsync();
        }

        await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Bank created successfully.");
        return Json(new { success = true });
      }

      return Json(new { success = false, message = "Error creating Bank. Please check the inputs." });
    }

    public async Task<IActionResult> Delete(int id)
    {
      var Bank = await _appDBContext.FI_Banks.FindAsync(id);
      if (Bank == null)
      {
        return NotFound();
      }

      Bank.ActiveYNID = 2;
      Bank.DeleteYNID = 1;

      _appDBContext.FI_Banks.Update(Bank);
      await _appDBContext.SaveChangesAsync();
      await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Bank deleted successfully.");

      return Json(new { success = true });
    }
    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var Banks = await _appDBContext.FI_Banks
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();

      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add("Banks");
        worksheet.Cells["A1"].Value = "Bank ID";
        worksheet.Cells["B1"].Value = "Bank Name";
        worksheet.Cells["C1"].Value = _localizer["lbl_Active"];
        worksheet.Cells["D1"].Value = "Account No";
        worksheet.Cells["E1"].Value = _localizer["lbl_Address"];


        for (int i = 0; i < Banks.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = Banks[i].BankID;
          worksheet.Cells[i + 2, 2].Value = Banks[i].BankName;
          worksheet.Cells[i + 2, 3].Value = Banks[i].ActiveYNID == 1 ? "Yes" : "No";
          worksheet.Cells[i + 2, 4].Value = Banks[i].AccountNo;
          worksheet.Cells[i + 2, 5].Value = Banks[i].Address;
        }

        worksheet.Cells["A1:l1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = $"Banks-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
    public async Task<IActionResult> Print()
    {
      var Banks = await _appDBContext.FI_Banks
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();
      return View("~/Views/Finance/Management/Bank/PrintBank.cshtml", Banks);
    }


  }
}

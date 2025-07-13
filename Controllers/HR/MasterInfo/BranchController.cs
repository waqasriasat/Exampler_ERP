using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml;
using System.Drawing.Printing;
using Microsoft.Extensions.Localization;

namespace Exampler_ERP.Controllers.HR.MasterInfo
{
  public class BranchController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IStringLocalizer<BranchController> _localizer;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    private readonly IHubContext<NotificationHub> _hubContext;


    public BranchController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext, IStringLocalizer<BranchController> localizer)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
      _hubContext = hubContext;
      _localizer = localizer;

    }
    public async Task<IActionResult> Index(string searchBranchName)
    {
      string plainText = "ibs";
      string encryptedText = CR_CipherKey.Encrypt(plainText);
      Console.WriteLine($"Encrypted: {encryptedText}");

      string plainText1 = "A@123456";
      string encryptedText1 = CR_CipherKey.Encrypt(plainText1);
      Console.WriteLine($"Encrypted: {encryptedText1}");

      var branchesQuery = _appDBContext.Settings_BranchTypes
          .Where(b => b.DeleteYNID != 1);

      if (!string.IsNullOrEmpty(searchBranchName))
      {
        branchesQuery = branchesQuery.Where(b => b.BranchTypeName.Contains(searchBranchName));
      }

      var branches = await branchesQuery.ToListAsync();

      if (!string.IsNullOrEmpty(searchBranchName) && branches.Count == 0)
      {
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "No branch found with the name '" + searchBranchName + "'. Please check the name and try again.");
      }
      return View("~/Views/HR/MasterInfo/Branch/Branch.cshtml", branches);
    }


    public async Task<IActionResult> Branch()
    {
      var Branchs = await _appDBContext.Settings_BranchTypes.ToListAsync();
      return Ok(Branchs);
    }// Add the Edit action
    public async Task<IActionResult> Edit(int id)
    {
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      var branch = await _appDBContext.Settings_BranchTypes.FindAsync(id);
      if (branch == null)
      {
        return NotFound();
      }
      return PartialView("~/Views/HR/MasterInfo/Branch/EditBranch.cshtml", branch);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Settings_BranchType branch)
    {
      if (ModelState.IsValid)
      {
        if (string.IsNullOrEmpty(branch.BranchTypeName))
        {
          return Json(new { success = false, message = "Branch Name field is required. Please enter a valid text value." });
        }


        _appDBContext.Update(branch);
        await _appDBContext.SaveChangesAsync();
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Branch updated successfully.");
        return Json(new { success = true });
      }
      return Json(new { success = false, message = "Error creating branch. Please check the inputs." });
    }
    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      return PartialView("~/Views/HR/MasterInfo/Branch/AddBranch.cshtml", new Settings_BranchType());
    }

    [HttpPost]
    public async Task<IActionResult> Create(Settings_BranchType branch)
    {
      if (ModelState.IsValid)
      {
        if (string.IsNullOrEmpty(branch.BranchTypeName))
        {
          return Json(new { success = false, message = "Branch Name field is required. Please enter a valid text value." });
        }


        branch.Date = DateTime.Now;
        branch.DeleteYNID = 0;

        _appDBContext.Settings_BranchTypes.Add(branch);
        await _appDBContext.SaveChangesAsync();

        await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Branch created successfully.");
        return Json(new { success = true });
      }

      return Json(new { success = false, message = "Error creating branch. Please check the inputs." });
    }

    public async Task<IActionResult> Delete(int id)
    {
      var branch = await _appDBContext.Settings_BranchTypes.FindAsync(id);
      if (branch == null)
      {
        return NotFound();
      }

      branch.ActiveYNID = 2;
      branch.DeleteYNID = 1;

      _appDBContext.Settings_BranchTypes.Update(branch);
      await _appDBContext.SaveChangesAsync();
      await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Branch deleted successfully.");

      return Json(new { success = true });
    }
    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var branches = await _appDBContext.Settings_BranchTypes
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();

      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add("Branches");
        worksheet.Cells["A1"].Value = _localizer["lbl_BranchID"];
        worksheet.Cells["B1"].Value = _localizer["lbl_Date"];
        worksheet.Cells["C1"].Value = _localizer["lbl_BranchName"];
        worksheet.Cells["D1"].Value = _localizer["lbl_Active"];
        worksheet.Cells["E1"].Value = _localizer["lbl_POBox"];
        worksheet.Cells["F1"].Value = _localizer["lbl_Country"];
        worksheet.Cells["G1"].Value = _localizer["lbl_City"];
        worksheet.Cells["H1"].Value = _localizer["lbl_Street"];
        worksheet.Cells["I1"].Value = _localizer["lbl_Phone"];
        worksheet.Cells["J1"].Value = _localizer["lbl_Fax"];
        worksheet.Cells["K1"].Value = _localizer["lbl_Mobile"];
        worksheet.Cells["L1"].Value = _localizer["lbl_Address"];


        for (int i = 0; i < branches.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = branches[i].BranchTypeID;
          worksheet.Cells[i + 2, 2].Value = branches[i].Date;
          worksheet.Cells[i + 2, 3].Value = branches[i].BranchTypeName;
          worksheet.Cells[i + 2, 4].Value = branches[i].ActiveYNID == 1 ? "Yes" : "No";
          worksheet.Cells[i + 2, 5].Value = branches[i].POBox;
          worksheet.Cells[i + 2, 6].Value = branches[i].Country;
          worksheet.Cells[i + 2, 7].Value = branches[i].City;
          worksheet.Cells[i + 2, 8].Value = branches[i].Street;
          worksheet.Cells[i + 2, 9].Value = branches[i].Phone;
          worksheet.Cells[i + 2, 10].Value = branches[i].Fax;
          worksheet.Cells[i + 2, 11].Value = branches[i].Mobile;
          worksheet.Cells[i + 2, 12].Value = branches[i].Address;
        }

        worksheet.Cells["A1:l1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = $"Branches-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
    public async Task<IActionResult> Print()
    {
      var branches = await _appDBContext.Settings_BranchTypes
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();
      return View("~/Views/HR/MasterInfo/Branch/PrintBranches.cshtml", branches);
    }


  }
}

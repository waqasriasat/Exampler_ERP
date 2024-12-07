using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace Exampler_ERP.Controllers.Finance.MasterInfo
{
  public class HeadofAccount_FourController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;

    public HeadofAccount_FourController(AppDBContext appDBContext, IConfiguration configuration, Utils utils)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
    }
    public async Task<IActionResult> Index(string searchFourName)
    {
      var HeadofAccount_FoursQuery = _appDBContext.Settings_HeadofAccount_Fours
          .Include(b => b.HeadofAccount_Third)
          .Where(b => b.DeleteYNID != 1);

      if (!string.IsNullOrEmpty(searchFourName))
      {
        HeadofAccount_FoursQuery = HeadofAccount_FoursQuery.Where(b => b.HeadofAccount_FourName.Contains(searchFourName));
      }

      var HeadofAccount_Fours = await HeadofAccount_FoursQuery.ToListAsync();

      if (!string.IsNullOrEmpty(searchFourName) && HeadofAccount_Fours.Count == 0)
      {
        TempData["ErrorMessage"] = "No HeadofAccount_Four Name found with the name '" + searchFourName + "'. Please check the name and try again.";
      }
      return View("~/Views/Finance/MasterInfo/HeadofAccount_Four/HeadofAccount_Four.cshtml", HeadofAccount_Fours);
    }


    public async Task<IActionResult> HeadofAccount_Four()
    {
      var HeadofAccount_Fours = await _appDBContext.Settings_HeadofAccount_Fours.ToListAsync();
      return Ok(HeadofAccount_Fours);
    }// Add the Edit action
    public async Task<IActionResult> Edit(int id)
    {
      ViewBag.ThirdList = await _utils.GetHeadofAccount_Third();
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      var HeadofAccount_Four = await _appDBContext.Settings_HeadofAccount_Fours.FindAsync(id);
      if (HeadofAccount_Four == null)
      {
        return NotFound();
      }
      return PartialView("~/Views/Finance/MasterInfo/HeadofAccount_Four/EditHeadofAccount_Four.cshtml", HeadofAccount_Four);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Settings_HeadofAccount_Four HeadofAccount_Four)
    {
      if (ModelState.IsValid)
      {
        if (string.IsNullOrEmpty(HeadofAccount_Four.HeadofAccount_FourName))
        {
          return Json(new { success = false, message = "HeadofAccount_Four Name field is required. Please enter a valid text value." });
        }


        _appDBContext.Update(HeadofAccount_Four);
        await _appDBContext.SaveChangesAsync();
        TempData["SuccessMessage"] = "HeadofAccount_Four Name updated successfully.";
        return Json(new { success = true });
      }
      return Json(new { success = false, message = "Error creating HeadofAccount_Four Name. Please check the inputs." });
    }
    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.ThirdList = await _utils.GetHeadofAccount_Third();
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      return PartialView("~/Views/Finance/MasterInfo/HeadofAccount_Four/AddHeadofAccount_Four.cshtml", new Settings_HeadofAccount_Four());
    }

    [HttpPost]
    public async Task<IActionResult> Create(Settings_HeadofAccount_Four HeadofAccount_Four)
    {
      if (ModelState.IsValid)
      {
        if (string.IsNullOrEmpty(HeadofAccount_Four.HeadofAccount_FourName))
        {
          return Json(new { success = false, message = "HeadofAccount_Four Name field is required. Please enter a valid text value." });
        }


        HeadofAccount_Four.DeleteYNID = 0;

        _appDBContext.Settings_HeadofAccount_Fours.Add(HeadofAccount_Four);
        await _appDBContext.SaveChangesAsync();

        TempData["SuccessMessage"] = "HeadofAccount_Four Name created successfully.";
        return Json(new { success = true });
      }

      return Json(new { success = false, message = "Error creating HeadofAccount_Four Name. Please check the inputs." });
    }

    public async Task<IActionResult> Delete(int id)
    {
      var HeadofAccount_Four = await _appDBContext.Settings_HeadofAccount_Fours.FindAsync(id);
      if (HeadofAccount_Four == null)
      {
        return NotFound();
      }

      HeadofAccount_Four.ActiveYNID = 2;
      HeadofAccount_Four.DeleteYNID = 1;

      _appDBContext.Settings_HeadofAccount_Fours.Update(HeadofAccount_Four);
      await _appDBContext.SaveChangesAsync();
      TempData["SuccessMessage"] = "HeadofAccount_Four Name deleted successfully.";

      return Json(new { success = true });
    }
    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var HeadofAccount_Fours = await _appDBContext.Settings_HeadofAccount_Fours
          .Include(b => b.HeadofAccount_Third)
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();

      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add("HeadofAccount_Fours");
        worksheet.Cells["A1"].Value = "HeadofAccount_Four ID";
        worksheet.Cells["B1"].Value = "HeadofAccount_Four Name";
        worksheet.Cells["C1"].Value = "Active";


        for (int i = 0; i < HeadofAccount_Fours.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = HeadofAccount_Fours[i].HeadofAccount_FourID;
          worksheet.Cells[i + 2, 2].Value = HeadofAccount_Fours[i].HeadofAccount_FourName;
          worksheet.Cells[i + 2, 3].Value = HeadofAccount_Fours[i].ActiveYNID == 1 ? "Yes" : "No";
        }

        worksheet.Cells["A1:l1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = $"HeadofAccount_Fours-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
    public async Task<IActionResult> Print()
    {
      var HeadofAccount_Fours = await _appDBContext.Settings_HeadofAccount_Fours
          .Include(b => b.HeadofAccount_Third)
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();
      return View("~/Views/Finance/MasterInfo/HeadofAccount_Four/PrintHeadofAccount_Four.cshtml", HeadofAccount_Fours);
    }


  }
}

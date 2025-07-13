using Exampler_ERP.Hubs;
using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;
using OfficeOpenXml;
using static System.Collections.Specialized.BitVector32;
using Microsoft.Extensions.Localization;

namespace Exampler_ERP.Controllers.HR.MasterInfo
{
  public class SubQualificationController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IStringLocalizer<SubQualificationController> _localizer;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    private readonly IHubContext<NotificationHub> _hubContext;


    public SubQualificationController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext, IStringLocalizer<SubQualificationController> localizer)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
      _hubContext = hubContext;
      _localizer = localizer;

    }
    public async Task<IActionResult> Index(string searchSubQualificationName)
    {

      var SubQualificationsQuery = _appDBContext.Settings_SubQualificationTypes
          .Where(b => b.DeleteYNID != 1);

      if (!string.IsNullOrEmpty(searchSubQualificationName))
      {
        SubQualificationsQuery = SubQualificationsQuery.Where(b => b.SubQualificationTypeName.Contains(searchSubQualificationName));
      }

      var SubQualifications = await SubQualificationsQuery.Include(d => d.QualificationType).ToListAsync();

      if (!string.IsNullOrEmpty(searchSubQualificationName) && SubQualifications.Count == 0)
      {
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "No SubQualification found with the name '" + searchSubQualificationName + "'. Please check the name and try again.");
      }

      return View("~/Views/HR/MasterInfo/SubQualification/SubQualification.cshtml", SubQualifications);
    }


    public async Task<IActionResult> SubQualification()
    {
      var SubQualification = await _appDBContext.Settings_SubQualificationTypes.ToListAsync();
      return Ok(SubQualification);
    }// Add the Edit action
    public async Task<IActionResult> Edit(int id)
    {
      ViewBag.QualificationList = await _utils.GetQualifications();
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      var SubQualification = await _appDBContext.Settings_SubQualificationTypes.FindAsync(id);
      if (SubQualification == null)
      {
        return NotFound();
      }
      return PartialView("~/Views/HR/MasterInfo/SubQualification/EditSubQualification.cshtml", SubQualification);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Settings_SubQualificationType SubQualification)
    {
      if (ModelState.IsValid)
      {
        if (string.IsNullOrEmpty(SubQualification.SubQualificationTypeName))
        {
          return Json(new { success = false, message = "SubQualification Name field is required. Please enter a valid text value." });
        }
        _appDBContext.Update(SubQualification);
        await _appDBContext.SaveChangesAsync();
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "SubQualification Updated successfully.");
        return Json(new { success = true });
      }
      return Json(new { success = false, message = "Error creating SubQualification. Please check the inputs." });
    }
    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.QualificationList = await _utils.GetQualifications();
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      return PartialView("~/Views/HR/MasterInfo/SubQualification/AddSubQualification.cshtml", new Settings_SubQualificationType());
    }

    [HttpPost]
    public async Task<IActionResult> Create(Settings_SubQualificationType SubQualification)
    {
      if (ModelState.IsValid)
      {
        if (string.IsNullOrEmpty(SubQualification.SubQualificationTypeName))
        {
          return Json(new { success = false, message = "SubQualification Name field is required. Please enter a valid text value." });
        }
        SubQualification.DeleteYNID = 0;
        _appDBContext.Settings_SubQualificationTypes.Add(SubQualification);
        await _appDBContext.SaveChangesAsync();
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "SubQualification Created successfully.");
        return Json(new { success = true });
      }
      return Json(new { success = false, message = "Error creating SubQualification. Please check the inputs." });
    }
    public async Task<IActionResult> Delete(int id)
    {
      var SubQualification = await _appDBContext.Settings_SubQualificationTypes.FindAsync(id);
      if (SubQualification == null)
      {
        return NotFound();
      }

      SubQualification.ActiveYNID = 2;
      SubQualification.DeleteYNID = 1;

      _appDBContext.Settings_SubQualificationTypes.Update(SubQualification);
      await _appDBContext.SaveChangesAsync();
      await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "SubQualification Deleted successfully.");
      return Json(new { success = true });
    }
    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var SubQualification = await _appDBContext.Settings_SubQualificationTypes
        .Where(b => b.DeleteYNID != 1)
        .Include(d => d.QualificationType) // Eagerly load the related Qualification data
        .ToListAsync();


      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add("SubQualification");
        worksheet.Cells["A1"].Value = "SubQualification ID";
        worksheet.Cells["B1"].Value = "Qualification Name";
        worksheet.Cells["C1"].Value = "SubQualification Name";
        worksheet.Cells["D1"].Value = "Active";


        for (int i = 0; i < SubQualification.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = SubQualification[i].SubQualificationTypeID;
          worksheet.Cells[i + 2, 2].Value = SubQualification[i].QualificationType?.QualificationTypeName;
          worksheet.Cells[i + 2, 3].Value = SubQualification[i].SubQualificationTypeName;
          worksheet.Cells[i + 2, 4].Value = SubQualification[i].ActiveYNID == 1 ? "Yes" : "No";
        }

        worksheet.Cells["A1:l1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = $"SubQualification-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
    public async Task<IActionResult> Print()
    {
      var SubQualification = await _appDBContext.Settings_SubQualificationTypes
        .Where(b => b.DeleteYNID != 1)
        .Include(d => d.QualificationType) // Eagerly load the related Qualification data
        .ToListAsync();
      return View("~/Views/HR/MasterInfo/SubQualification/PrintSubQualifications.cshtml", SubQualification);
    }

  }
}

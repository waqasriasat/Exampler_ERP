using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;
using OfficeOpenXml;
using Microsoft.Extensions.Localization;

namespace Exampler_ERP.Controllers.HR.MasterInfo
{
  public class ProcessTypeController : PositionController
  {
    private readonly AppDBContext _appDBContext;
    private readonly IStringLocalizer<ProcessTypeController> _localizer;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    private readonly IHubContext<NotificationHub> _hubContext;


    public ProcessTypeController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext, IStringLocalizer<ProcessTypeController> localizer) 
    : base(appDBContext)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
      _hubContext = hubContext;
      _localizer = localizer;

    }
    public async Task<IActionResult> Index(string searchProcessTypeName)
    {

      var ProcessTypesQuery = _appDBContext.Settings_ProcessTypes
          .Where(b => b.DeleteYNID != 1);

      if (!string.IsNullOrEmpty(searchProcessTypeName))
      {
        ProcessTypesQuery = ProcessTypesQuery.Where(b => b.ProcessTypeName.Contains(searchProcessTypeName));
      }

      var ProcessTypes = await ProcessTypesQuery.ToListAsync();

      if (!string.IsNullOrEmpty(searchProcessTypeName) && ProcessTypes.Count == 0)
      {
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "No Process Type found with the name '" + searchProcessTypeName + "'. Please check the name and try again.");
      }

      return View("~/Views/HR/MasterInfo/ProcessType/ProcessType.cshtml", ProcessTypes);
    }

    public async Task<IActionResult> ProcessType()
    {
      var ProcessTypes = await _appDBContext.Settings_ProcessTypes.ToListAsync();
      return Ok(ProcessTypes);
    }// Add the Edit action
    public async Task<IActionResult> Edit(int id)
    {
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      var ProcessType = await _appDBContext.Settings_ProcessTypes.FindAsync(id);
      if (ProcessType == null)
      {
        return NotFound();
      }
      return PartialView("~/Views/HR/MasterInfo/ProcessType/EditProcessType.cshtml", ProcessType);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Settings_ProcessType ProcessType)
    {
      if (ModelState.IsValid)
      {
        if (string.IsNullOrEmpty(ProcessType.ProcessTypeName))
        {
          return Json(new { success = false, message = "ProcessType Name field is required. Please enter a valid text value." });
        }
        _appDBContext.Update(ProcessType);
        await _appDBContext.SaveChangesAsync();
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Process Type Updated successfully.");
        return Json(new { success = true });
      }
      return Json(new { success = false, message = "Error creating ProcessType. Please check the inputs." });
    }
    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      return PartialView("~/Views/HR/MasterInfo/ProcessType/AddProcessType.cshtml", new Settings_ProcessType());
    }

    [HttpPost]
    public async Task<IActionResult> Create(Settings_ProcessType ProcessType)
    {
      if (ModelState.IsValid)
      {
        if (string.IsNullOrEmpty(ProcessType.ProcessTypeName))
        {
          return Json(new { success = false, message = "ProcessType Name field is required. Please enter a valid text value." });
        }
        ProcessType.DeleteYNID = 0;
        _appDBContext.Settings_ProcessTypes.Add(ProcessType);
        await _appDBContext.SaveChangesAsync();
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Process Type Created successfully.");
        return Json(new { success = true });
      }
      return Json(new { success = false, message = "Error creating ProcessType. Please check the inputs." });
    }
    public async Task<IActionResult> Delete(int id)
    {
      var ProcessType = await _appDBContext.Settings_ProcessTypes.FindAsync(id);
      if (ProcessType == null)
      {
        return NotFound();
      }

      ProcessType.ActiveYNID = 0;
      ProcessType.DeleteYNID = 1;

      _appDBContext.Settings_ProcessTypes.Update(ProcessType);
      await _appDBContext.SaveChangesAsync();
      await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Process Type Deleted successfully.");
      return Json(new { success = true });
    }
    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var ProcessTypees = await _appDBContext.Settings_ProcessTypes
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();

      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add(_localizer["lbl_ProcessType"]);
        worksheet.Cells["A1"].Value = _localizer["lbl_ProcessTypeID"];
        worksheet.Cells["B1"].Value = _localizer["lbl_ProcessTypeName"];
        worksheet.Cells["C1"].Value = _localizer["lbl_Active"];


        for (int i = 0; i < ProcessTypees.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = ProcessTypees[i].ProcessTypeID;
          worksheet.Cells[i + 2, 2].Value = ProcessTypees[i].ProcessTypeName;
          worksheet.Cells[i + 2, 3].Value = ProcessTypees[i].ActiveYNID == 1 ? _localizer["lbl_Yes"] : _localizer["lbl_No"];
        }

        worksheet.Cells["A1:C1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = _localizer["lbl_ProcessType"]+$"-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
    public async Task<IActionResult> Print()
    {
      var ProcessTypees = await _appDBContext.Settings_ProcessTypes
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();
      return View("~/Views/HR/MasterInfo/ProcessType/PrintProcessTypes.cshtml", ProcessTypees);
    }

  }
}

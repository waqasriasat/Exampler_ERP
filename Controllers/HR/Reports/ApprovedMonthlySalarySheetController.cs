using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Localization;
using OfficeOpenXml;

namespace Exampler_ERP.Controllers.HR.Reports
{
  public class ApprovedMonthlySalarySheetController : PositionController
  {
    private readonly AppDBContext _appDBContext;
    private readonly IStringLocalizer<ApprovedMonthlySalarySheetController> _localizer;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    private readonly IHubContext<NotificationHub> _hubContext;

    public ApprovedMonthlySalarySheetController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext, IStringLocalizer<ApprovedMonthlySalarySheetController> localizer) 
    : base(appDBContext)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
      _hubContext = hubContext;
      _localizer = localizer;

    }
    public async Task<IActionResult> Index(int? Branch, int? MonthsTypeID, int? YearsTypeID)
    {
      if (!Branch.HasValue || !MonthsTypeID.HasValue || !YearsTypeID.HasValue)
      {
        var today = DateTime.Today;
        //Branch ??= 1;
        MonthsTypeID ??= today.Month;
        YearsTypeID ??= today.Year;
      }
      int branchValue = Branch ?? 0;

      var query = _appDBContext.HR_MonthlyPayrolls
        .Include(b => b.BranchType)
        .Include(b => b.MonthType)
        .AsQueryable();

      if (MonthsTypeID.HasValue && MonthsTypeID != 0 && YearsTypeID.HasValue && YearsTypeID != 0)
      {
        query = query.Where(b => b.MonthTypeID == MonthsTypeID.Value && b.Year == YearsTypeID.Value);
      }

      if (Branch.HasValue && Branch != 0)
      {
        query = query.Where(b => b.BranchTypeID == Branch.Value);
      }
   
      var monthlyPayroll = await query.ToListAsync();

      await PopulateDropdowns(MonthsTypeID, YearsTypeID, Branch);

      return View("~/Views/HR/Reports/ApprovedMonthlySalarySheet/ApprovedMonthlySalarySheet.cshtml", monthlyPayroll);
    }
    private async Task PopulateDropdowns(int? month, int? year, int? Branch)
    {
      ViewBag.MonthsTypeID = month;
      ViewBag.YearsTypeID = year;
      ViewBag.Branch = Branch;

      ViewBag.MonthsTypeList = await _utils.GetMonthsTypes();
      ViewBag.BranchList = await _utils.GetBranchs();
    }
    public async Task<IActionResult> Print(int? Branch, int? MonthsTypeID, int? YearsTypeID)
    {
      if (!Branch.HasValue || !MonthsTypeID.HasValue || !YearsTypeID.HasValue)
      {
        var today = DateTime.Today;
        //Branch ??= 1;
        MonthsTypeID ??= today.Month;
        YearsTypeID ??= today.Year;
      }
      int branchValue = Branch ?? 0;

      var query = _appDBContext.HR_MonthlyPayrolls
        .Include(b => b.BranchType)
        .Include(b => b.MonthType)
        .AsQueryable();

      if (MonthsTypeID.HasValue && MonthsTypeID != 0 && YearsTypeID.HasValue && YearsTypeID != 0)
      {
        query = query.Where(b => b.MonthTypeID == MonthsTypeID.Value && b.Year == YearsTypeID.Value);
      }

      if (Branch.HasValue && Branch != 0)
      {
        query = query.Where(b => b.BranchTypeID == Branch.Value);
      }

      var monthlyPayroll = await query.ToListAsync();

      await PopulateDropdowns(MonthsTypeID, YearsTypeID, Branch);

      return View("~/Views/HR/Reports/ApprovedMonthlySalarySheet/PrintApprovedMonthlySalarySheet.cshtml", monthlyPayroll);
    }
    public async Task<IActionResult> ExportToExcel(int? Branch, int? MonthsTypeID, int? YearsTypeID)
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      if (!Branch.HasValue || !MonthsTypeID.HasValue || !YearsTypeID.HasValue)
      {
        var today = DateTime.Today;
        //Branch ??= 1;
        MonthsTypeID ??= today.Month;
        YearsTypeID ??= today.Year;
      }
      int branchValue = Branch ?? 0;

      var query = _appDBContext.HR_MonthlyPayrolls
        .Include(b => b.BranchType)
        .Include(b => b.MonthType)
        .AsQueryable();

      if (MonthsTypeID.HasValue && MonthsTypeID != 0 && YearsTypeID.HasValue && YearsTypeID != 0)
      {
        query = query.Where(b => b.MonthTypeID == MonthsTypeID.Value && b.Year == YearsTypeID.Value);
      }

      if (Branch.HasValue && Branch != 0)
      {
        query = query.Where(b => b.BranchTypeID == Branch.Value);
      }

      var monthlyPayroll = await query.ToListAsync();

      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add(_localizer["lbl_ApprovedMonthlySalarySheetList"]);
        worksheet.Cells["A1"].Value = _localizer["lbl_BranchName"];
        worksheet.Cells["B1"].Value = _localizer["lbl_Month"];
        worksheet.Cells["C1"].Value = _localizer["lbl_Year"];


        for (int i = 0; i < monthlyPayroll.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = monthlyPayroll[i].BranchType?.BranchTypeName;
          worksheet.Cells[i + 2, 2].Value = monthlyPayroll[i].MonthType?.MonthTypeName;
          worksheet.Cells[i + 2, 3].Value = monthlyPayroll[i].Year;
        }

        worksheet.Cells["A1:l1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = _localizer["lbl_ApprovedMonthlySalarySheetList"] + $"-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
  }
}

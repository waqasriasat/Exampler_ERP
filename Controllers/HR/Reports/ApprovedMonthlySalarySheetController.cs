using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Localization;

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
      var query = _appDBContext.HR_MonthlyPayrolls
        .Include(b => b.BranchType)
        .Include(b => b.MonthType)
        .AsQueryable();

      if (Branch.HasValue && Branch != 0)
      {
        query = query.Where(b => b.BranchTypeID == Branch.Value);
      }
      if (MonthsTypeID.HasValue && MonthsTypeID != 0)
      {
        query = query.Where(b => b.MonthTypeID == MonthsTypeID.Value);
      }
      if (YearsTypeID.HasValue)
      {
        query = query.Where(b => b.Year == YearsTypeID.Value);
      }

      var monthlyPayroll = await query.ToListAsync();

      ViewBag.MonthsTypeID = MonthsTypeID;
      ViewBag.YearsTypeID = YearsTypeID;
      ViewBag.BranchID = Branch;

      ViewBag.MonthsTypeList = await _utils.GetMonthsTypes();
      ViewBag.BranchList = await _utils.GetBranchs();


      return View("~/Views/HR/Reports/ApprovedMonthlySalarySheet/ApprovedMonthlySalarySheet.cshtml", monthlyPayroll);
    }

  }
}

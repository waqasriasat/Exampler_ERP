using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Localization;

namespace Exampler_ERP.Controllers.EmployeePortal.Main
{
  public class EmployeeApply_DashboardController : PositionController
  {
    private readonly AppDBContext _appDBContext;
    private readonly IStringLocalizer<EmployeeApply_DashboardController> _localizer;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    private readonly IHubContext<NotificationHub> _hubContext;



    public EmployeeApply_DashboardController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext, IStringLocalizer<EmployeeApply_DashboardController> localizer) 
    : base(appDBContext)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
      _hubContext = hubContext;
      _localizer = localizer;

    }
    public async Task<IActionResult> Index()
    {
      ViewBag.VacationCount = await _appDBContext.HR_Vacations.CountAsync();
      ViewBag.EmployeeRequestCount = await _appDBContext.HR_EmployeeRequestTypeApprovals.CountAsync();
      ViewBag.DocumentsUploadCount = await _appDBContext.HR_DocumentUploads.CountAsync();
      //ViewBag.SalaryCount = await _appDBContext.HR_Salarys.CountAsync();
      //ViewBag.JoiningCount = await _appDBContext.HR_Joinings.CountAsync();
      //ViewBag.BankAccountCount = await _appDBContext.HR_BankAccounts.CountAsync();
      //ViewBag.ContractRenewwalCount = await _appDBContext.HR_ContractRenewals.CountAsync();
      //ViewBag.LeaveBalanceCount = await _appDBContext.leave.CountAsync();
      //ViewBag.CardPrintCount = await _appDBContext.Card.CountAsync();

      return View("~/Views/EmployeePortal/Main/Dashboard/EmployeeApply_Dashboard.cshtml");
    }
    public IActionResult GetEmployeePicture(int employeeID)
    {
      var employeePicture = _appDBContext.HR_Employees
                      .Where(emp => emp.EmployeeID == employeeID)
                      .Select(emp => emp.Picture)
                      .FirstOrDefault();

      if (employeePicture != null)
      {
        return File(employeePicture, "image/jpeg"); // or "image/png" depending on your image format
      }
      else
      {
        var noImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/icons/NoImage.jpg");
        var noImageFile = System.IO.File.ReadAllBytes(noImagePath);
        return File(noImageFile, "image/jpeg");
      }
    }
  }
}

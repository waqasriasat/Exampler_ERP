using Exampler_ERP.Models.Temp;
using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR;
using Exampler_ERP.Hubs;
using Microsoft.Extensions.Localization;
using OfficeOpenXml;

namespace Exampler_ERP.Controllers.HR.HR
{
  public class AdditionalAllowanceController : PositionController
  {
    private readonly AppDBContext _appDBContext;
    private readonly IStringLocalizer<AdditionalAllowanceController> _localizer;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AdditionalAllowanceController> _logger;
    private readonly Utils _utils;
    private readonly IHubContext<NotificationHub> _hubContext;
    public AdditionalAllowanceController(AppDBContext appDBContext, IConfiguration configuration, ILogger<AdditionalAllowanceController> logger, Utils utils, IHubContext<NotificationHub> hubContext, IStringLocalizer<AdditionalAllowanceController> localizer) 
    : base(appDBContext)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _logger = logger;
      _utils = utils;
      _hubContext = hubContext;
      _localizer = localizer;


    }
    public async Task<IActionResult> Index(int? MonthsTypeID, int? YearsTypeID, string? EmployeeName, int? EmployeeID)
    {
      var query = _appDBContext.HR_AdditionalAllowances
        .Include(d => d.Employee)
        .Include(d => d.MonthType)
        .AsQueryable();

      if (MonthsTypeID.HasValue && MonthsTypeID != 0)
      {
        query = query.Where(d => d.MonthTypeID == MonthsTypeID.Value);
      }

      if (YearsTypeID.HasValue)
      {
        query = query.Where(d => d.Year == YearsTypeID.Value);
      }

      if (EmployeeID.HasValue)
      {
        query = query.Where(d => d.EmployeeID == EmployeeID.Value);
      }

      if (!string.IsNullOrEmpty(EmployeeName))
      {
        query = query.Where(d =>
            (d.Employee.FirstName + " " + d.Employee.FatherName + " " + d.Employee.FamilyName)
            .Contains(EmployeeName));
      }
      var AdditionalAllowances = await query.ToListAsync();

      ViewBag.MonthsTypeID = MonthsTypeID;
      ViewBag.YearsTypeID = YearsTypeID;
      ViewBag.EmployeeID = EmployeeID;
      ViewBag.EmployeeName = EmployeeName;

      ViewBag.MonthsTypeList = await _utils.GetMonthsTypes();

      return View("~/Views/HR/HR/AdditionalAllowance/AdditionalAllowance.cshtml", AdditionalAllowances);
    }
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
      var allowance = await _appDBContext.HR_AdditionalAllowances
          .Include(a => a.AdditionalAllowanceDetails)
          .FirstOrDefaultAsync(a => a.AdditionalAllowanceID == id);

      if (allowance == null)
      {
        return NotFound();
      }
      if (allowance.PostedID != null && allowance.PostedID != 0)
      {
        //await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "This deduction has already been posted to the Payroll Department and cannot be edited..");
        return NotFound();
      }
      ViewBag.AdditionalAllowanceTypeList = await _appDBContext.Settings_AdditionalAllowanceTypes
         .Select(r => new { Value = r.AdditionalAllowanceTypeID, Text = r.AdditionalAllowanceTypeName })
         .ToListAsync();

      ViewBag.EmployeesList = await _utils.GetEmployee();
      ViewBag.MonthsList = await _utils.GetMonthsTypes();

      return PartialView("~/Views/HR/HR/AdditionalAllowance/EditAdditionalAllowance.cshtml", allowance);
    }
    [HttpPost]
    public async Task<IActionResult> Edit(HR_AdditionalAllowance model)
    {
      if (ModelState.IsValid)
      {
        var existingAllowance = await _appDBContext.HR_AdditionalAllowances
            .Include(a => a.AdditionalAllowanceDetails)
            .FirstOrDefaultAsync(a => a.AdditionalAllowanceID == model.AdditionalAllowanceID);

        if (existingAllowance == null)
        {
          return NotFound();
        }

        existingAllowance.EmployeeID = model.EmployeeID;
        existingAllowance.MonthTypeID = model.MonthTypeID;
        existingAllowance.Year = model.Year;

        foreach (var existingDetail in existingAllowance.AdditionalAllowanceDetails.ToList())
        {
          var updatedDetail = model.AdditionalAllowanceDetails
               .FirstOrDefault(d => d.AdditionalAllowanceDetailID == existingDetail.AdditionalAllowanceDetailID);

          if (updatedDetail == null)
          {
            _appDBContext.HR_AdditionalAllowanceDetails.Remove(existingDetail);
          }
          else
          {
            existingDetail.AdditionalAllowanceTypeID = updatedDetail.AdditionalAllowanceTypeID;
            existingDetail.AdditionalAllowanceAmount = updatedDetail.AdditionalAllowanceAmount;
          }
        }

        foreach (var newDetail in model.AdditionalAllowanceDetails)
        {
          if (newDetail.AdditionalAllowanceDetailID == 0)
          {
            existingAllowance.AdditionalAllowanceDetails.Add(newDetail);
          }
        }

        await _appDBContext.SaveChangesAsync();
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Additional Allowance updated successfully.");
        return Json(new { success = true });
      }

      ViewBag.AdditionalAllowanceTypeList = await _appDBContext.Settings_AdditionalAllowanceTypes
          .Select(r => new { Value = r.AdditionalAllowanceTypeID, Text = r.AdditionalAllowanceTypeName })
          .ToListAsync();

      ViewBag.EmployeesList = await _utils.GetEmployee();
      ViewBag.MonthsList = await _utils.GetMonthsTypes();
      await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "Error updating Additional Allowance. Please check the inputs.");
      return PartialView("~/Views/HR/HR/AdditionalAllowance/EditAdditionalAllowance.cshtml", model);
    }
    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.AdditionalAllowanceTypeList = await _appDBContext.Settings_AdditionalAllowanceTypes
          .Select(r => new { Value = r.AdditionalAllowanceTypeID, Text = r.AdditionalAllowanceTypeName })
          .ToListAsync();

      ViewBag.EmployeesList = await _utils.GetEmployee();
      ViewBag.MonthsList = await _utils.GetMonthsTypes();

      return PartialView("~/Views/HR/HR/AdditionalAllowance/AddAdditionalAllowance.cshtml", new HR_AdditionalAllowance());
    }
    [HttpPost]
    public async Task<IActionResult> Create(HR_AdditionalAllowance model)
    {

      if (ModelState.IsValid)
      {
        _appDBContext.HR_AdditionalAllowances.Add(model);
        await _appDBContext.SaveChangesAsync();

        int generatedAdditionalAllowanceID = model.AdditionalAllowanceID;
        if (generatedAdditionalAllowanceID > 0)
        {
          var processCount = await _appDBContext.CR_ProcessTypeApprovalSetups
                              .Where(pta => pta.ProcessTypeID > 0 && pta.ProcessTypeID == 8)
                              .CountAsync();
          var getEmployeeID = await _appDBContext.HR_AdditionalAllowances
                            .Where(pta => pta.AdditionalAllowanceID == generatedAdditionalAllowanceID)
                            .FirstOrDefaultAsync();
          if (processCount > 0)
          {
            var newProcessTypeApproval = new CR_ProcessTypeApproval
            {
              ProcessTypeID = 8,
              Notes = "Additional Allowance",
              Date = DateTime.Now,
              EmployeeID = getEmployeeID.EmployeeID,
              UserID = HttpContext.Session.GetInt32("UserID") ?? default(int),
              TransactionID = generatedAdditionalAllowanceID
            };

            _appDBContext.CR_ProcessTypeApprovals.Add(newProcessTypeApproval);
            await _appDBContext.SaveChangesAsync();

            var nextApprovalSetup = await _appDBContext.CR_ProcessTypeApprovalSetups
                                        .Where(pta => pta.ProcessTypeID == 8 && pta.Rank == 1)
                                        .FirstOrDefaultAsync();

            if (nextApprovalSetup != null)
            {
              var newProcessTypeApprovalDetail = new CR_ProcessTypeApprovalDetail
              {
                ProcessTypeApprovalID = newProcessTypeApproval.ProcessTypeApprovalID,
                Date = DateTime.Now,
                RoleID = nextApprovalSetup.RoleTypeID,
                AppID = 0,
                AppUserID = 0,
                Notes = null,
                Rank = nextApprovalSetup.Rank
              };

              _appDBContext.CR_ProcessTypeApprovalDetails.Add(newProcessTypeApprovalDetail);
              await _appDBContext.SaveChangesAsync();
              await _hubContext.Clients.All.SendAsync("ReceiveProcessNotification");
            }
            else
            {
              return Json(new { success = false, message = "Next approval setup not found." });
            }
          }
          else
          {
            model.FinalApprovalID = 1;
            _appDBContext.HR_AdditionalAllowances.Update(model);
            await _appDBContext.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Additional Allowance Created successfully. No process setup found, Additional Allowance activated.");
          }
        }
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Additional Allowance Created successfully. Continue to the Approval Process Setup for Additional Allowance Activation.");
        return Json(new { success = true });
      }

      ViewBag.AdditionalAllowanceTypeList = await _appDBContext.Settings_AdditionalAllowanceTypes
          .Select(r => new { Value = r.AdditionalAllowanceTypeID, Text = r.AdditionalAllowanceTypeName })
          .ToListAsync();
      ViewBag.EmployeesList = await _utils.GetEmployee();
      ViewBag.MonthsList = await _utils.GetMonthsTypes();
      await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "Error creating Additional Allowance. Please check the inputs.");
      return PartialView("~/Views/HR/HR/AdditionalAllowance/AddAdditionalAllowance.cshtml", model);
    }
    public async Task<IActionResult> Delete(int id)
    {
      var allowance = await _appDBContext.HR_AdditionalAllowances.FindAsync(id);
      if (allowance == null)
      {
        return NotFound();
      }

      _appDBContext.HR_AdditionalAllowances.Remove(allowance);
      await _appDBContext.SaveChangesAsync();
      await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Additional Allowance Deleted successfully.");
      return Json(new { success = true });
    }
    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var addAllowance = _appDBContext.HR_AdditionalAllowances
        .Include(d => d.Employee)
        .Include(d => d.MonthType)
        .ToListAsync();

      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add(_localizer["lbl_AdditionalAllowance"]);
        worksheet.Cells["A1"].Value = _localizer["lbl_SalaryTypeID"];
        worksheet.Cells["B1"].Value = _localizer["lbl_SalaryTypeName"];
        worksheet.Cells["C1"].Value = _localizer["lbl_Active"];


        for (int i = 0; i < addAllowance.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = addAllowance[i].SalaryTypeID;
          worksheet.Cells[i + 2, 2].Value = addAllowance[i].SalaryTypeName;
          worksheet.Cells[i + 2, 3].Value = addAllowance[i].ActiveYNID == 1 ? _localizer["lbl_Yes"] : _localizer["lbl_No"];
        }

        worksheet.Cells["A1:C1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = _localizer["lbl_AdditionalAllowance"] + $"-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
    public async Task<IActionResult> Print()
    {
      var SalaryTypees = await _appDBContext.Settings_SalaryTypes
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();
      return View("~/Views/HR/MasterInfo/SalaryType/PrintSalaryTypes.cshtml", SalaryTypees);
    }
  }
}

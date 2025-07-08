using Exampler_ERP.Models.Temp;
using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR;
using Exampler_ERP.Hubs;

namespace Exampler_ERP.Controllers.HR.HR
{
  public class AddionalAllowanceController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AddionalAllowanceController> _logger;
    private readonly Utils _utils;
private readonly IHubContext<NotificationHub> _hubContext;

    
    public AddionalAllowanceController(AppDBContext appDBContext, IConfiguration configuration, ILogger<AddionalAllowanceController> logger, Utils utils, IHubContext<NotificationHub> hubContext)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _logger = logger;
      _utils = utils;
_hubContext = hubContext;
 
      
    }
    public async Task<IActionResult> Index(int? MonthsTypeID, int? YearsTypeID, string? EmployeeName, int? EmployeeID)
    {
      var query = _appDBContext.HR_AddionalAllowances
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
      var addionalAllowances = await query.ToListAsync();

      ViewBag.MonthsTypeID = MonthsTypeID;
      ViewBag.YearsTypeID = YearsTypeID;
      ViewBag.EmployeeID = EmployeeID;
      ViewBag.EmployeeName = EmployeeName;

      ViewBag.MonthsTypeList = await _utils.GetMonthsTypes();

      return View("~/Views/HR/HR/AddionalAllowance/AddionalAllowance.cshtml", addionalAllowances);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
      var allowance = await _appDBContext.HR_AddionalAllowances
          .Include(a => a.AddionalAllowanceDetails)
          .FirstOrDefaultAsync(a => a.AddionalAllowanceID == id );

      if (allowance == null)
      {
        return NotFound();
      }
      if (allowance.PostedID != null && allowance.PostedID != 0)
      {
        //await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "This deduction has already been posted to the Payroll Department and cannot be edited..");
        return NotFound();
      }
       ViewBag.AddionalAllowanceTypeList = await _appDBContext.Settings_AddionalAllowanceTypes
          .Select(r => new { Value = r.AddionalAllowanceTypeID, Text = r.AddionalAllowanceTypeName })
          .ToListAsync();

      ViewBag.EmployeesList = await _utils.GetEmployee();
      ViewBag.MonthsList = await _utils.GetMonthsTypes();

      return PartialView("~/Views/HR/HR/AddionalAllowance/EditAddionalAllowance.cshtml", allowance);
    }
    [HttpPost]
    public async Task<IActionResult> Edit(HR_AddionalAllowance model)
    {
      if (ModelState.IsValid)
      {
        var existingAllowance = await _appDBContext.HR_AddionalAllowances
            .Include(a => a.AddionalAllowanceDetails)
            .FirstOrDefaultAsync(a => a.AddionalAllowanceID == model.AddionalAllowanceID);

        if (existingAllowance == null)
        {
          return NotFound();
        }

        existingAllowance.EmployeeID = model.EmployeeID;
        existingAllowance.MonthTypeID = model.MonthTypeID;
        existingAllowance.Year = model.Year;

        foreach (var existingDetail in existingAllowance.AddionalAllowanceDetails.ToList())
        {
          var updatedDetail = model.AddionalAllowanceDetails
               .FirstOrDefault(d => d.AddionalAllowanceDetailID == existingDetail.AddionalAllowanceDetailID);

          if (updatedDetail == null)
          {
            _appDBContext.HR_AddionalAllowanceDetails.Remove(existingDetail);
          }
          else
          {
            existingDetail.AddionalAllowanceTypeID = updatedDetail.AddionalAllowanceTypeID;
            existingDetail.AddionalAllowanceAmount = updatedDetail.AddionalAllowanceAmount;
          }
        }

        foreach (var newDetail in model.AddionalAllowanceDetails)
        {
          if (newDetail.AddionalAllowanceDetailID == 0)
          {
            existingAllowance.AddionalAllowanceDetails.Add(newDetail);
          }
        }

        await _appDBContext.SaveChangesAsync();
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Addional Allowance updated successfully.");
        return Json(new { success = true });
      }

      ViewBag.AddionalAllowanceTypeList = await _appDBContext.Settings_AddionalAllowanceTypes
          .Select(r => new { Value = r.AddionalAllowanceTypeID, Text = r.AddionalAllowanceTypeName })
          .ToListAsync();

      ViewBag.EmployeesList = await _utils.GetEmployee();
      ViewBag.MonthsList = await _utils.GetMonthsTypes();
      await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "Error updating Addional Allowance. Please check the inputs.");
      return PartialView("~/Views/HR/HR/AddionalAllowance/EditAddionalAllowance.cshtml", model);
    }



    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.AddionalAllowanceTypeList = await _appDBContext.Settings_AddionalAllowanceTypes
          .Select(r => new { Value = r.AddionalAllowanceTypeID, Text = r.AddionalAllowanceTypeName })
          .ToListAsync();

      ViewBag.EmployeesList = await _utils.GetEmployee();
      ViewBag.MonthsList = await _utils.GetMonthsTypes();

      return PartialView("~/Views/HR/HR/AddionalAllowance/AddAddionalAllowance.cshtml", new HR_AddionalAllowance());
    }

    [HttpPost]
    public async Task<IActionResult> Create(HR_AddionalAllowance model)
    {

      if (ModelState.IsValid)
      {
        _appDBContext.HR_AddionalAllowances.Add(model);
        await _appDBContext.SaveChangesAsync();

        int generatedAddionalAllowanceID = model.AddionalAllowanceID;
        if (generatedAddionalAllowanceID > 0)
        {
          var processCount = await _appDBContext.CR_ProcessTypeApprovalSetups
                              .Where(pta => pta.ProcessTypeID > 0 && pta.ProcessTypeID == 8)
                              .CountAsync();
          var getEmployeeID = await _appDBContext.HR_AddionalAllowances
                            .Where(pta => pta.AddionalAllowanceID == generatedAddionalAllowanceID)
                            .FirstOrDefaultAsync();
          if (processCount > 0)
          {
            var newProcessTypeApproval = new CR_ProcessTypeApproval
            {
              ProcessTypeID = 8,
              Notes = "Addional Allowance",
              Date = DateTime.Now,
              EmployeeID = getEmployeeID.EmployeeID,
              UserID = HttpContext.Session.GetInt32("UserID") ?? default(int),
              TransactionID = generatedAddionalAllowanceID
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
            _appDBContext.HR_AddionalAllowances.Update(model);
            await _appDBContext.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Addional Allowance Created successfully. No process setup found, Addional Allowance activated.");
          }
        }
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Addional Allowance Created successfully. Continue to the Approval Process Setup for Addional Allowance Activation.");
        return Json(new { success = true });
      }

      ViewBag.AddionalAllowanceTypeList = await _appDBContext.Settings_AddionalAllowanceTypes
          .Select(r => new { Value = r.AddionalAllowanceTypeID, Text = r.AddionalAllowanceTypeName })
          .ToListAsync();
      ViewBag.EmployeesList = await _utils.GetEmployee();
      ViewBag.MonthsList = await _utils.GetMonthsTypes();
      await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "Error creating Addional Allowance. Please check the inputs.");
      return PartialView("~/Views/HR/HR/AddionalAllowance/AddAddionalAllowance.cshtml", model);
    }


  }
}

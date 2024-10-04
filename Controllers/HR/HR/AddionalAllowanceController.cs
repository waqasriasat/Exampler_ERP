using Exampler_ERP.Models.Temp;
using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Exampler_ERP.Controllers.HR.HR
{
  public class AddionalAllowanceController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AddionalAllowanceController> _logger;
    private readonly Utils _utils;
    public AddionalAllowanceController(AppDBContext appDBContext, IConfiguration configuration, ILogger<AddionalAllowanceController> logger, Utils utils)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _logger = logger;
      _utils = utils;
    }
    public async Task<IActionResult> Index()
    {
      var addionalAllowances = await _appDBContext.HR_AddionalAllowances
        .Include(d => d.Employee)
        .Include(d => d.MonthType)
        .ToListAsync();
      

      return View("~/Views/HR/HR/AddionalAllowance/AddionalAllowance.cshtml", addionalAllowances);
    }

    public async Task<IActionResult> Edit(int id)
    {
      var AddionalAllowanceDetails = await _appDBContext.HR_AddionalAllowanceDetails
       .Where(pt => pt.AddionalAllowance.EmployeeID == id)
       .Include(pt => pt.AddionalAllowance.Employee)
       .ToListAsync();

      if (AddionalAllowanceDetails == null || !AddionalAllowanceDetails.Any())
      {
        var employee = await _appDBContext.HR_Employees
            .Where(p => p.EmployeeID == id)
            .FirstOrDefaultAsync();

        if (employee != null)
        {
          // Create a new AddionalAllowanceDetails list with an initial entry
          AddionalAllowanceDetails = new List<HR_AddionalAllowanceDetail>
            {
                new HR_AddionalAllowanceDetail
                {
                    AddionalAllowance = new HR_AddionalAllowance
                    {
                            Employee = employee
                    }
                }
            };
        }
        else
        {
          return NotFound(); // Handle accordingly if the Employee is not found
        }
      }

      ViewBag.AddionalAllowanceTypeList = await _appDBContext.Settings_AddionalAllowanceTypes
          .Select(r => new { Value = r.AddionalAllowanceTypeID, Text = r.AddionalAllowanceTypeName })
          .ToListAsync();


      return PartialView("~/Views/HR/HR/AddionalAllowance/EditAddionalAllowance.cshtml", AddionalAllowanceDetails);

    }

    [HttpPost]
    public async Task<IActionResult> Edit(int EmployeeID, List<HR_AddionalAllowanceDetail> AddionalAllowanceDetails)
    {
      foreach (var setup in AddionalAllowanceDetails)
      {
        _logger.LogInformation("Received Setup: ID={AddionalAllowanceID}, AddionalAllowanceTypeID={AddionalAllowanceTypeID}, AddionalAllowanceAmount={AddionalAllowanceAmount}", setup.AddionalAllowanceID, setup.AddionalAllowanceTypeID, setup.AddionalAllowanceAmount);
      }

      if (AddionalAllowanceDetails == null || AddionalAllowanceDetails.Count == 0)
      {
        _logger.LogWarning("No data received for edit.");
        return Json(new { success = false, message = "No data received." });
      }

      if (ModelState.IsValid)
      {
        try
        {
          var AddionalAllowanceId = AddionalAllowanceDetails.FirstOrDefault()?.AddionalAllowanceID;
          int generatedAddionalAllowanceID;
          int getemployeeID;

          if (AddionalAllowanceId != null && AddionalAllowanceId != 0)
          {
            var existingRecords = await _appDBContext.HR_AddionalAllowanceDetails
                                        .Where(p => p.AddionalAllowanceID == AddionalAllowanceId)
                                        .ToListAsync();

            _appDBContext.HR_AddionalAllowanceDetails.RemoveRange(existingRecords);
            generatedAddionalAllowanceID = AddionalAllowanceId.Value;
          }
          else
          {

            var AddionalAllowance = new HR_AddionalAllowance()
            {
              EmployeeID = EmployeeID,
              FinalApprovalID = 0,
              ApprovalProcessID = 0
            };
            _appDBContext.HR_AddionalAllowances.Add(AddionalAllowance);
            await _appDBContext.SaveChangesAsync();

            generatedAddionalAllowanceID = AddionalAllowance.AddionalAllowanceID;
          }

          foreach (var setup in AddionalAllowanceDetails)
          {
            if (setup.AddionalAllowanceTypeID != 0 && setup.AddionalAllowanceAmount != 0)
            {
              setup.AddionalAllowanceID = generatedAddionalAllowanceID;
              _appDBContext.HR_AddionalAllowanceDetails.Add(setup);
            }
          }

          await _appDBContext.SaveChangesAsync();


          if (generatedAddionalAllowanceID > 0)
          {
            var processCount = await _appDBContext.CR_ProcessTypeApprovalSetups
                                .Where(pta => pta.ProcessTypeID > 0 && pta.ProcessTypeID == 6)
                                .CountAsync();
            var getEmployeeID = await _appDBContext.HR_AddionalAllowances
                              .Where(pta => pta.AddionalAllowanceID == generatedAddionalAllowanceID)
                              .FirstOrDefaultAsync();
            if (processCount > 0)
            {
              var newProcessTypeApproval = new CR_ProcessTypeApproval
              {
                ProcessTypeID = 6,
                Notes = "Employee AddionalAllowance",
                Date = DateTime.Now,
                EmployeeID = getEmployeeID.EmployeeID,
                UserID = HttpContext.Session.GetInt32("UserID") ?? default(int),
                TransactionID = generatedAddionalAllowanceID
              };

              _appDBContext.CR_ProcessTypeApprovals.Add(newProcessTypeApproval);
              await _appDBContext.SaveChangesAsync();

              var nextApprovalSetup = await _appDBContext.CR_ProcessTypeApprovalSetups
                                          .Where(pta => pta.ProcessTypeID == 6 && pta.Rank == 1)
                                          .FirstOrDefaultAsync();

              if (nextApprovalSetup != null)
              {
                var newProcessTypeApprovalDetail = new CR_ProcessTypeApprovalDetail
                {
                  ApprovalProcessID = newProcessTypeApproval.ApprovalProcessID,
                  Date = DateTime.Now,
                  RoleID = nextApprovalSetup.RoleTypeID,
                  AppID = 0,
                  AppUserID = 0,
                  Notes = null,
                  Rank = nextApprovalSetup.Rank
                };

                _appDBContext.CR_ProcessTypeApprovalDetails.Add(newProcessTypeApprovalDetail);
                await _appDBContext.SaveChangesAsync();
              }
              else
              {
                return Json(new { success = false, message = "Next approval setup not found." });
              }
            }
          }

          return Json(new { success = true });
        }
        catch (Exception ex)
        {
          _logger.LogError(ex, "Error updating AddionalAllowanceDetails");
          return Json(new { success = false, message = "An error occurred while updating the data." });
        }
      }

      var errors = ModelState.Values.SelectMany(v => v.Errors);
      return PartialView("~/Views/HR/HR/AddionalAllowance/EditAddionalAllowance.cshtml", AddionalAllowanceDetails);
    }
    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.AddionalAllowanceTypeList = await _appDBContext.Settings_AddionalAllowanceTypes
          .Select(r => new { Value = r.AddionalAllowanceTypeID, Text = r.AddionalAllowanceTypeName })
          .ToListAsync();

      ViewBag.EmployeesList = await _utils.GetEmployee(); // Assuming this method fetches employee data
      ViewBag.MonthsList = await _utils.GetMonthsTypes(); // Assuming this method fetches month data

      return PartialView("~/Views/HR/HR/AddionalAllowance/AddAddionalAllowance.cshtml", new HR_AddionalAllowance());
    }

    [HttpPost]
    public async Task<IActionResult> Create(HR_AddionalAllowance model)
    {
      if (ModelState.IsValid)
      {
        _appDBContext.HR_AddionalAllowances.Add(model);
        await _appDBContext.SaveChangesAsync();

        if (model.AddionalAllowanceDetails != null && model.AddionalAllowanceDetails.Any())
        {
          foreach (var detail in model.AddionalAllowanceDetails)
          {
            detail.AddionalAllowanceID = model.AddionalAllowanceID; // Set the foreign key
            _appDBContext.HR_AddionalAllowanceDetails.Add(detail);
          }

          await _appDBContext.SaveChangesAsync();
        }

     
        return PartialView("~/Views/HR/HR/AddionalAllowance/AddAddionalAllowance.cshtml", model);
      }

      ViewBag.AddionalAllowanceTypeList = await _appDBContext.Settings_AddionalAllowanceTypes
          .Select(r => new { Value = r.AddionalAllowanceTypeID, Text = r.AddionalAllowanceTypeName })
          .ToListAsync();
      ViewBag.EmployeesList = await _utils.GetEmployee();
      ViewBag.MonthsList = await _utils.GetMonthsTypes();

      return PartialView("~/Views/HR/HR/AddionalAllowance/AddAddionalAllowance.cshtml", model);
    }



  }
}

using Exampler_ERP.Models.Temp;
using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Contracts;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Exampler_ERP.Controllers.HR.Financial
{
  public class FixedDeductionController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly ILogger<FixedDeductionController> _logger;
    private readonly Utils _utils;
    public FixedDeductionController(AppDBContext appDBContext, IConfiguration configuration, ILogger<FixedDeductionController> logger, Utils utils)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _logger = logger;
      _utils = utils;
    }
    public async Task<IActionResult> Index(int? id)
    {
      var FixedDeductiontypes = await _appDBContext.Settings_FixedDeductionTypes.ToListAsync();
      var FixedDeductions = await _appDBContext.HR_FixedDeductions.ToListAsync();
      var FixedDeductionDetails = await _appDBContext.HR_FixedDeductionDetails.ToListAsync();

      var employeesQuery = from emp in _appDBContext.HR_Employees
                           join con in _appDBContext.HR_Contracts
                           on emp.EmployeeID equals con.EmployeeID
                           where con.ActiveYNID == 1 && emp.ActiveYNID == 1
                           select emp;

      if (id.HasValue)
      {
        employeesQuery = employeesQuery.Where(e => e.EmployeeID == id);
      }

      var employees = await employeesQuery.ToListAsync();

      var employeeIds = employees.Select(e => e.EmployeeID).ToList();
      var deductionCounts = await _appDBContext.HR_FixedDeductionDetails
          .Where(sd => employeeIds.Contains(sd.FixedDeduction.EmployeeID))
          .GroupBy(sd => sd.FixedDeduction.EmployeeID)
          .Select(g => new { EmployeeID = g.Key, Count = g.Count() })
          .ToListAsync();

      var employeeCounts = employees.Select(emp => new EmployeeCountViewModel
      {
        EmployeeID = emp.EmployeeID,
        EmployeeName = emp.FirstName + " " + emp.FatherName + " " + emp.FamilyName,
        TypeCount = deductionCounts.FirstOrDefault(dc => dc.EmployeeID == emp.EmployeeID)?.Count ?? 0
      }).ToList();

      var viewModel = new EmployeeListViewModel
      {
        EmployeeCount = employeeCounts
      };

      return View("~/Views/HR/Financial/FixedDeduction/FixedDeduction.cshtml", viewModel);
    }


    public async Task<IActionResult> Edit(int id)
    {
      var FixedDeductionDetails = await _appDBContext.HR_FixedDeductionDetails
       .Where(pt => pt.FixedDeduction.EmployeeID == id)
       .Include(pt => pt.FixedDeduction.Employee)
       .ToListAsync();

      if (FixedDeductionDetails == null || !FixedDeductionDetails.Any())
      {
        var employee = await _appDBContext.HR_Employees
            .Where(p => p.EmployeeID == id)
            .FirstOrDefaultAsync();

        if (employee != null)
        {
          // Create a new FixedDeductionDetails list with an initial entry
          FixedDeductionDetails = new List<HR_FixedDeductionDetail>
            {
                new HR_FixedDeductionDetail
                {
                    FixedDeduction = new HR_FixedDeduction
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

      ViewBag.FixedDeductionTypeList = await _appDBContext.Settings_FixedDeductionTypes
          .Select(r => new { Value = r.FixedDeductionTypeID, Text = r.FixedDeductionTypeName })
          .ToListAsync();


      return PartialView("~/Views/HR/Financial/FixedDeduction/EditFixedDeduction.cshtml", FixedDeductionDetails);

    }

    [HttpPost]
    public async Task<IActionResult> Edit(int EmployeeID, List<HR_FixedDeductionDetail> FixedDeductionDetails)
    {
      foreach (var setup in FixedDeductionDetails)
      {
        _logger.LogInformation("Received Setup: ID={FixedDeductionID}, FixedDeductionTypeID={FixedDeductionTypeID}, FixedDeductionAmount={FixedDeductionAmount}", setup.FixedDeductionID, setup.FixedDeductionTypeID, setup.FixedDeductionAmount);
      }

      if (FixedDeductionDetails == null || FixedDeductionDetails.Count == 0)
      {
        _logger.LogWarning("No data received for edit.");
        return Json(new { success = false, message = "No data received." });
      }

      if (ModelState.IsValid)
      {
        try
        {
          var FixedDeductionId = FixedDeductionDetails.FirstOrDefault()?.FixedDeductionID;
          int generatedFixedDeductionID;
          int getemployeeID;

          if (FixedDeductionId != null && FixedDeductionId != 0)
          {
            var existingRecords = await _appDBContext.HR_FixedDeductionDetails
                                        .Where(p => p.FixedDeductionID == FixedDeductionId)
                                        .ToListAsync();

            _appDBContext.HR_FixedDeductionDetails.RemoveRange(existingRecords);
            generatedFixedDeductionID = FixedDeductionId.Value;
          }
          else
          {

            var FixedDeduction = new HR_FixedDeduction()
            {
              EmployeeID = EmployeeID,
              FinalApprovalID = 0,
              ApprovalProcessID = 0
            };
            _appDBContext.HR_FixedDeductions.Add(FixedDeduction);
            await _appDBContext.SaveChangesAsync();

            generatedFixedDeductionID = FixedDeduction.FixedDeductionID;


            foreach (var setup in FixedDeductionDetails)
            {
              if (setup.FixedDeductionTypeID != 0 && setup.FixedDeductionAmount != 0)
              {
                setup.FixedDeductionID = generatedFixedDeductionID;
                _appDBContext.HR_FixedDeductionDetails.Add(setup);
              }
            }

            await _appDBContext.SaveChangesAsync();


            if (generatedFixedDeductionID > 0)
            {
              var processCount = await _appDBContext.CR_ProcessTypeApprovalSetups
                                  .Where(pta => pta.ProcessTypeID > 0 && pta.ProcessTypeID == 10)
                                  .CountAsync();
              var getEmployeeID = await _appDBContext.HR_FixedDeductions
                                .Where(pta => pta.FixedDeductionID == generatedFixedDeductionID)
                                .FirstOrDefaultAsync();
              if (processCount > 0)
              {
                var newProcessTypeApproval = new CR_ProcessTypeApproval
                {
                  ProcessTypeID = 10,
                  Notes = "Fixed Deduction",
                  Date = DateTime.Now,
                  EmployeeID = getEmployeeID.EmployeeID,
                  UserID = HttpContext.Session.GetInt32("UserID") ?? default(int),
                  TransactionID = generatedFixedDeductionID
                };

                _appDBContext.CR_ProcessTypeApprovals.Add(newProcessTypeApproval);
                await _appDBContext.SaveChangesAsync();

                var nextApprovalSetup = await _appDBContext.CR_ProcessTypeApprovalSetups
                                            .Where(pta => pta.ProcessTypeID == 10 && pta.Rank == 1)
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
              else
              {
                FixedDeduction.FinalApprovalID = 1;
                _appDBContext.HR_FixedDeductions.Update(FixedDeduction);
                await _appDBContext.SaveChangesAsync();
                return Json(new { success = true, message = "No process setup found, User activated." });
              }
            }
          }

          return Json(new { success = true });
        }
        catch (Exception ex)
        {
          _logger.LogError(ex, "Error updating FixedDeductionDetails");
          return Json(new { success = false, message = "An error occurred while updating the data." });
        }
      }

      var errors = ModelState.Values.SelectMany(v => v.Errors);
      return PartialView("~/Views/HR/Financial/FixedDeduction/EditFixedDeduction.cshtml", FixedDeductionDetails);
    }



  }
}

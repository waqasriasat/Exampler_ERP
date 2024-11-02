using Exampler_ERP.Models;
using Exampler_ERP.Models.Temp;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Diagnostics.Contracts;
using Contract = Exampler_ERP.Models.HR_Contract;

namespace Exampler_ERP.Controllers.HR.Employeement
{
   public class SalaryController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly ILogger<SalaryController> _logger;
    private readonly Utils _utils;
    public SalaryController(AppDBContext appDBContext, IConfiguration configuration, ILogger<SalaryController> logger, Utils utils)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _logger = logger;
      _utils = utils;
    }
    public async Task<IActionResult> Index(int? id) // EmployeeID
    {
      var salarytypes = await _appDBContext.Settings_SalaryTypes.ToListAsync();
      var salarys = await _appDBContext.HR_Salarys.ToListAsync();
      var salaryDetails = await _appDBContext.HR_SalaryDetails.ToListAsync();

      // Filter employees based on id (if provided)
      var employeesQuery = from emp in _appDBContext.HR_Employees
                           join con in _appDBContext.HR_Contracts
                           on emp.EmployeeID equals con.EmployeeID
                           where con.ActiveYNID == 1 && emp.ActiveYNID == 1
                           select emp;

      if (id.HasValue)
      {
        employeesQuery = employeesQuery.Where(e => e.EmployeeID == id.Value);
      }

      var employees = await employeesQuery.ToListAsync();

      var employeeCounts = new List<EmployeeCountViewModel>();

      foreach (var pt in employees)
      {
        var typeCount = await _appDBContext.HR_SalaryDetails
            .Where(sd => sd.Salary.EmployeeID == pt.EmployeeID)
            .CountAsync();

        employeeCounts.Add(new EmployeeCountViewModel
        {
          EmployeeID = pt.EmployeeID,
          EmployeeName = pt.FirstName + " " + pt.FatherName + " " + pt.FamilyName,
          TypeCount = typeCount
        });
      }

      var viewModel = new EmployeeListViewModel
      {
        EmployeeCount = employeeCounts
      };


      return View("~/Views/HR/Employeement/Salary/Salary.cshtml", viewModel);
    }


    public async Task<IActionResult> Edit(int id)
    {
      var SalaryDetails = await _appDBContext.HR_SalaryDetails
       .Where(pt => pt.Salary.EmployeeID == id)
       .Include(pt => pt.Salary.Employee)
       .ToListAsync();

      if (SalaryDetails == null || !SalaryDetails.Any())
      {
        var employee = await _appDBContext.HR_Employees
            .Where(p => p.EmployeeID == id)
            .FirstOrDefaultAsync();

        if (employee != null)
        {
          // Create a new SalaryDetails list with an initial entry
          SalaryDetails = new List<HR_SalaryDetail>
            {
                new HR_SalaryDetail
                {
                    Salary = new HR_Salary
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

      ViewBag.SalaryTypeList = await _appDBContext.Settings_SalaryTypes
          .Select(r => new { Value = r.SalaryTypeID, Text = r.SalaryTypeName })
          .ToListAsync();

   
      return PartialView("~/Views/HR/Employeement/Salary/EditSalary.cshtml", SalaryDetails);

    }

    [HttpPost]
    public async Task<IActionResult> Edit(int EmployeeID, List<HR_SalaryDetail> SalaryDetails)
    {
      foreach (var setup in SalaryDetails)
      {
        _logger.LogInformation("Received Setup: ID={SalaryID}, SalaryTypeID={SalaryTypeID}, SalaryAmount={SalaryAmount}", setup.SalaryID, setup.SalaryTypeID, setup.SalaryAmount);
      }

      if (SalaryDetails == null || SalaryDetails.Count == 0)
      {
        TempData["ErrorMessage"] = "No data received.";
        _logger.LogWarning("No data received for edit.");
        return Json(new { success = false, message = "No data received." });
      }

      if (ModelState.IsValid)
      {
        try
        {
          var SalaryId = SalaryDetails.FirstOrDefault()?.SalaryID;
          int generatedSalaryID;
          int getemployeeID;

          if (SalaryId != null && SalaryId != 0)
          {
            var existingRecords = await _appDBContext.HR_SalaryDetails
                                        .Where(p => p.SalaryID == SalaryId)
                                        .ToListAsync();

            _appDBContext.HR_SalaryDetails.RemoveRange(existingRecords);
            await _appDBContext.SaveChangesAsync();


            var salary = new HR_Salary()
            {
              EmployeeID = EmployeeID,
              FinalApprovalID = 0,
              ProcessTypeApprovalID = 0
            };
            _appDBContext.HR_Salarys.Add(salary);
            await _appDBContext.SaveChangesAsync();

            generatedSalaryID = salary.SalaryID;

            foreach (var setup in SalaryDetails)
            {
              if (setup.SalaryTypeID != 0 && setup.SalaryAmount != 0)
              {
                setup.SalaryID = generatedSalaryID;
                _appDBContext.HR_SalaryDetails.Add(setup);
              }
            }

            await _appDBContext.SaveChangesAsync();


            if (generatedSalaryID > 0)
            {
              var processCount = await _appDBContext.CR_ProcessTypeApprovalSetups
                                  .Where(pta => pta.ProcessTypeID > 0 && pta.ProcessTypeID == 6)
                                  .CountAsync();
              var getEmployeeID = await _appDBContext.HR_Salarys
                                .Where(pta => pta.SalaryID == generatedSalaryID)
                                .FirstOrDefaultAsync();
              if (processCount > 0)
              {
                var newProcessTypeApproval = new CR_ProcessTypeApproval
                {
                  ProcessTypeID = 6,
                  Notes = "Employee Salary",
                  Date = DateTime.Now,
                  EmployeeID = getEmployeeID.EmployeeID,
                  UserID = HttpContext.Session.GetInt32("UserID") ?? default(int),
                  TransactionID = generatedSalaryID
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
                }
                else
                {
                  return Json(new { success = false, message = "Next approval setup not found." });
                }
              }
              else
              {
                salary.FinalApprovalID = 1;
                _appDBContext.HR_Salarys.Update(salary);
                await _appDBContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "Salary Created successfully. No process setup found, Salary activated.";
                return Json(new { success = true});
              }
            }
          }
          else
          {

            var salary = new HR_Salary()
            {
              EmployeeID = EmployeeID,
              FinalApprovalID = 0,
              ProcessTypeApprovalID = 0
            };
            _appDBContext.HR_Salarys.Add(salary);
            await _appDBContext.SaveChangesAsync();

            generatedSalaryID = salary.SalaryID;

            foreach (var setup in SalaryDetails)
            {
              if (setup.SalaryTypeID != 0 && setup.SalaryAmount != 0)
              {
                setup.SalaryID = generatedSalaryID;
                _appDBContext.HR_SalaryDetails.Add(setup);
              }
            }

            await _appDBContext.SaveChangesAsync();


            if (generatedSalaryID > 0)
            {
              var processCount = await _appDBContext.CR_ProcessTypeApprovalSetups
                                  .Where(pta => pta.ProcessTypeID > 0 && pta.ProcessTypeID == 6)
                                  .CountAsync();
              var getEmployeeID = await _appDBContext.HR_Salarys
                                .Where(pta => pta.SalaryID == generatedSalaryID)
                                .FirstOrDefaultAsync();
              if (processCount > 0)
              {
                var newProcessTypeApproval = new CR_ProcessTypeApproval
                {
                  ProcessTypeID = 6,
                  Notes = "Employee Salary",
                  Date = DateTime.Now,
                  EmployeeID = getEmployeeID.EmployeeID,
                  UserID = HttpContext.Session.GetInt32("UserID") ?? default(int),
                  TransactionID = generatedSalaryID
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
                }
                else
                {
                  return Json(new { success = false, message = "Next approval setup not found." });
                }
              }
              else
              {
                salary.FinalApprovalID = 1;
                _appDBContext.HR_Salarys.Update(salary);
                await _appDBContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "Salary Created successfully. No process setup found, Salary activated.";
                return Json(new { success = true, message = "No process setup found, User activated." });
              }
            }
          }

          TempData["SuccessMessage"] = "Salary Created successfully. Continue to the Approval Process Setup for Salary Activation.";
          return Json(new { success = true });
        }
        catch (Exception ex)
        {
          TempData["ErrorMessage"] = "Error creating Salary. Please check the inputs.";
          _logger.LogError(ex, "Error updating SalaryDetails");
          return Json(new { success = false, message = "An error occurred while updating the data." });
        }
      }
      TempData["ErrorMessage"] = "Error creating Salary. Please check the inputs.";
      var errors = ModelState.Values.SelectMany(v => v.Errors);
      return PartialView("~/Views/HR/Employeement/Salary/EditSalary.cshtml", SalaryDetails);
    }

   

  }
}

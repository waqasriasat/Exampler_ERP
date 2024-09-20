using Exampler_ERP.Models.Temp;
using Exampler_ERP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Utilities;

namespace Exampler_ERP.Controllers.HR.Employeement
{
  public class JoiningController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly ILogger<JoiningController> _logger;
    private readonly Utils _utils;
    public JoiningController(AppDBContext appDBContext, IConfiguration configuration, ILogger<JoiningController> logger, Utils utils)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _logger = logger;
      _utils = utils;
    }
    public async Task<IActionResult> Index()
    {
      var employeeJoinings = await (from emp in _appDBContext.HR_Employees
                                    join con in _appDBContext.HR_Contracts
                                    on emp.EmployeeID equals con.EmployeeID
                                    join jon in _appDBContext.HR_Joinings
                                    on emp.EmployeeID equals jon.EmployeeID into joinGroup
                                    from j in joinGroup.DefaultIfEmpty()
                                    where con.ActiveYNID == 1 && emp.ActiveYNID == 1
                                    select new
                                    {
                                      emp.EmployeeID,
                                      emp.FirstName,
                                      emp.FatherName,
                                      emp.FamilyName,
                                      JoiningDate = j != null ? j.JoiningDate : (DateTime?)null
                                    }).ToListAsync();

      var employeeCounts = employeeJoinings.Select(ej => new EmployeeJoiningViewModel
      {
        EmployeeID = ej.EmployeeID,
        EmployeeName = $"{ej.FirstName} {ej.FatherName} {ej.FamilyName}",
        JoiningDate = ej.JoiningDate
      }).ToList();

      var viewModel = new EmployeeJoiningListViewModel
      {
        EmployeeJoining = employeeCounts
      };

      return View("~/Views/HR/Employeement/Joining/Joining.cshtml", viewModel);
    }

    public async Task<IActionResult> Edit(int id)
    {
      var existingJoining = await _appDBContext.HR_Joinings
          .Where(pt => pt.EmployeeID == id)
          .Include(pt => pt.Employee)
          .FirstOrDefaultAsync();

      if (existingJoining != null)
      {
        var joinings = new List<HR_Joining> { existingJoining };
        return PartialView("~/Views/HR/Employeement/Joining/EditJoining.cshtml", joinings);
      }
      else
      {
        var employee = await _appDBContext.HR_Employees
            .Where(p => p.EmployeeID == id)
            .FirstOrDefaultAsync();

        if (employee != null)
        {
          var joiningDate = await _appDBContext.HR_Joinings
              .Where(j => j.EmployeeID == id)
              .Select(j => j.JoiningDate)
              .FirstOrDefaultAsync();

          var joinings = new List<HR_Joining>
            {
                new HR_Joining
                {
                    Employee = employee,
                    JoiningDate = joiningDate // Use the retrieved JoiningDate
                }
            };

          return PartialView("~/Views/HR/Employeement/Joining/EditJoining.cshtml", joinings);
        }
        else
        {
          return NotFound(); // Employee not found
        }
      }
    }


    [HttpPost]
    public async Task<IActionResult> Edit(HR_Joining updatedJoining)
    {
      if (ModelState.IsValid)
      {
        try
        {
          var existingJoining = await _appDBContext.HR_Joinings
    .FirstOrDefaultAsync(j => j.EmployeeID == updatedJoining.EmployeeID);

          if (existingJoining != null)
          {
            // Update existing joining
            existingJoining.JoiningDate = updatedJoining.JoiningDate;
            _appDBContext.HR_Joinings.Update(existingJoining);
          }
          else
          {
            // Add new joining record
            await _appDBContext.HR_Joinings.AddAsync(updatedJoining);

            // Create new workday record
            var workday = new HR_WorkDay
            {
              EmployeeID = updatedJoining.EmployeeID,
              Month = updatedJoining.JoiningDate.Month,
              Year = updatedJoining.JoiningDate.Year,
              FromDate = new DateTime(updatedJoining.JoiningDate.Year, updatedJoining.JoiningDate.Month, 1),
              ToDate = updatedJoining.JoiningDate.AddDays(-1),
              Days = updatedJoining.JoiningDate.Day - 1,
              DeleteYNID = 0
            };

            await _appDBContext.HR_WorkDays.AddAsync(workday);

            // Update employee's status if the employee exists
            var employee = await _appDBContext.HR_Employees
                                              .FirstOrDefaultAsync(e => e.EmployeeID == updatedJoining.EmployeeID);

            if (employee != null)
            {
              employee.EmployeeStatusTypeID = 1; // Set status to 'Working' (assuming ID 1 means 'Working')
            }
          }

          // Save all changes to the database
          await _appDBContext.SaveChangesAsync();

          return Json(new { success = true });

        }
        catch (Exception ex)
        {
          _logger.LogError(ex, "Error saving joining changes");
          return Json(new { success = false, message = "An error occurred while saving changes." });
        }
      }
      return Json(new { success = false, message = "Invalid model state." });
    }


  }
}

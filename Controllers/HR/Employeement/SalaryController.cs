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
    public async Task<IActionResult> Index()
    {
      var salarytypes = await _appDBContext.Settings_SalaryTypes.ToListAsync();
      var salarys = await _appDBContext.HR_Salarys.ToListAsync();
      var salaryDetails = await _appDBContext.HR_SalaryDetails.ToListAsync();
      var employees = await (from emp in _appDBContext.HR_Employees
                             join con in _appDBContext.HR_Contracts
                             on emp.EmployeeID equals con.EmployeeID
                             where con.ActiveID == 1 && emp.ActiveID == 1
                             select emp).ToListAsync();

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
        _logger.LogWarning("No data received for edit.");
        return Json(new { success = false, message = "No data received." });
      }

      if (ModelState.IsValid)
      {
        try
        {
          var SalaryId = SalaryDetails.FirstOrDefault()?.SalaryID;
          int generatedSalaryID;

          if (SalaryId != null && SalaryId != 0)
          {
            var existingRecords = await _appDBContext.HR_SalaryDetails
                                        .Where(p => p.SalaryID == SalaryId)
                                        .ToListAsync();

            _appDBContext.HR_SalaryDetails.RemoveRange(existingRecords);
            generatedSalaryID = SalaryId.Value;
          }
          else
          {
        
            var salary = new HR_Salary()
            {
              EmployeeID = EmployeeID,
              FinalApprovalID = 0,
              ApprovalProcessID = 0
            };
            _appDBContext.HR_Salarys.Add(salary);
            await _appDBContext.SaveChangesAsync();

            generatedSalaryID = salary.SalaryID;
          }

          foreach (var setup in SalaryDetails)
          {
            if (setup.SalaryTypeID != 0 && setup.SalaryAmount != 0)
            {
              setup.SalaryID = generatedSalaryID;
              _appDBContext.HR_SalaryDetails.Add(setup);
            }
          }

          await _appDBContext.SaveChangesAsync();
          return Json(new { success = true });
        }
        catch (Exception ex)
        {
          _logger.LogError(ex, "Error updating SalaryDetails");
          return Json(new { success = false, message = "An error occurred while updating the data." });
        }
      }

      var errors = ModelState.Values.SelectMany(v => v.Errors);
      return PartialView("~/Views/HR/Employeement/Salary/EditSalary.cshtml", SalaryDetails);
    }

   

  }
}

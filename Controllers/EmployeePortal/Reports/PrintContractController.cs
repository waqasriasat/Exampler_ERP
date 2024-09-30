using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Exampler_ERP.Controllers.EmployeePortal.Reports
{
  public class PrintContractController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    public PrintContractController(AppDBContext appDBContext, IConfiguration configuration, Utils utils)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
    }
  
    public async Task<IActionResult> Index()
    {
      var employeeID = HttpContext.Session.GetInt32("EmployeeID");

  
      // Fetch the contract details based on the EmployeeID
      var contract = await _appDBContext.HR_Contracts
                                        .Include(c => c.Settings_ContractType)
                                        .Include(c => c.Employee)  // Assuming Contract has a relationship with Employee
                                        .Include(c => c.Employee.Department)
                                        .Include(c => c.Employee.Designation)
                                        .FirstOrDefaultAsync(c => c.EmployeeID == employeeID);

      if (contract == null)
      {
        // Return 404 if no contract is found
        return NotFound();
      }
      return View("~/Views/EmployeePortal/Reports/PrintContract/PrintContract.cshtml", contract);
    }
  }
}

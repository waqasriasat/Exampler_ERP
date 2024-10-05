using Exampler_ERP.Models.Temp;
using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Exampler_ERP.Controllers.HR.Reports
{
  public class ContractExpiryController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    public ContractExpiryController(AppDBContext appDBContext, IConfiguration configuration, Utils utils)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
    }
    public async Task<IActionResult> Index()
    {
      var contracts = await _appDBContext.HR_Contracts
        .Include(c => c.Employee)
        .Include(c => c.Employee.BranchType)
        .Include(c => c.Employee.DepartmentType)
        .Include(c => c.Settings_ContractType)
        .Select(c => new ContractEndDaysViewModel
        {
          EmployeeName = c.Employee.FirstName +' '+ c.Employee.FatherName + ' ' + c.Employee.FamilyName,
          BranchName = c.Employee.BranchType.BranchTypeName,
          DepartmentName = c.Employee.DepartmentType.DepartmentTypeName,
          ContractTypeName = c.Settings_ContractType.ContractTypeName,
          EndDays = c.EndDate.HasValue ? (c.EndDate.Value - DateTime.Now).Days : 0
        }).ToListAsync();

      return View("~/Views/HR/Reports/ContractExpiry/ContractExpiry.cshtml", contracts);
    }




  }
}

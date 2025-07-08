using Exampler_ERP.Models.Temp;
using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Exampler_ERP.Controllers.HR.Reports
{
  public class ContractExpiryController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
private readonly IHubContext<NotificationHub> _hubContext;

    public ContractExpiryController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
_hubContext = hubContext;
 
    }
 
    public async Task<IActionResult> Index(int? id)
    {
      var contractsQuery = _appDBContext.HR_Contracts
        .Where(c => c.DeleteYNID != 1 && c.FinalApprovalID == 1)
          .Include(c => c.Employee)
          .ThenInclude(e => e.BranchType)
          .Include(c => c.Employee)
          .ThenInclude(e => e.DepartmentType)
          .Include(c => c.Settings_ContractType)
          .AsQueryable();

      if (id.HasValue)
      {
        contractsQuery = contractsQuery.Where(c => c.Employee.EmployeeID == id.Value);
      }

      var contracts = await contractsQuery
          .Select(c => new ContractEndDaysViewModel
          {
            EmployeeName = c.Employee.FirstName + ' ' + c.Employee.FatherName + ' ' + c.Employee.FamilyName,
            BranchName = c.Employee.BranchType.BranchTypeName,
            DepartmentName = c.Employee.DepartmentType.DepartmentTypeName,
            ContractTypeName = c.Settings_ContractType.ContractTypeName,
            EndDays = c.EndDate.HasValue ? (c.EndDate.Value - DateTime.Now).Days : 0
          })
          .ToListAsync();

      return View("~/Views/HR/Reports/ContractExpiry/ContractExpiry.cshtml", contracts);
    }



  }
}

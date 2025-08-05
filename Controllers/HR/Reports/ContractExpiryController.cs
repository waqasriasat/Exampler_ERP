using Exampler_ERP.Models.Temp;
using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Localization;
using OfficeOpenXml;

namespace Exampler_ERP.Controllers.HR.Reports
{
  public class ContractExpiryController : PositionController
  {
    private readonly AppDBContext _appDBContext;
    private readonly IStringLocalizer<ContractExpiryController> _localizer;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    private readonly IHubContext<NotificationHub> _hubContext;

    public ContractExpiryController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext, IStringLocalizer<ContractExpiryController> localizer) 
    : base(appDBContext)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
      _hubContext = hubContext;
      _localizer = localizer;

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
    public async Task<IActionResult> Print()
    {
      var contractsQuery = _appDBContext.HR_Contracts
        .Where(c => c.DeleteYNID != 1 && c.FinalApprovalID == 1)
          .Include(c => c.Employee)
          .ThenInclude(e => e.BranchType)
          .Include(c => c.Employee)
          .ThenInclude(e => e.DepartmentType)
          .Include(c => c.Settings_ContractType)
          .AsQueryable();

    
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

      return View("~/Views/HR/Reports/ContractExpiry/PrintContractExpiry.cshtml", contracts);
    }

    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
      var contractsQuery = _appDBContext.HR_Contracts
        .Where(c => c.DeleteYNID != 1 && c.FinalApprovalID == 1)
          .Include(c => c.Employee)
          .ThenInclude(e => e.BranchType)
          .Include(c => c.Employee)
          .ThenInclude(e => e.DepartmentType)
          .Include(c => c.Settings_ContractType)
          .AsQueryable();


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

      var contractTypesList = await _utils.GetContractTypes();
      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add(_localizer["lbl_ContractExpiryList"]);

        worksheet.Cells["A1"].Value = _localizer["lbl_EmployeeName"];
        worksheet.Cells["B1"].Value = _localizer["lbl_Branch"];
        worksheet.Cells["C1"].Value = _localizer["lbl_Department"];
        worksheet.Cells["D1"].Value = _localizer["lbl_ContractType"];
        worksheet.Cells["E1"].Value = _localizer["lbl_RemainingEndDays"];

        for (int i = 0; i < contracts.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = contracts[i].EmployeeName;
          worksheet.Cells[i + 2, 2].Value = contracts[i].BranchName;
          worksheet.Cells[i + 2, 3].Value = contracts[i].DepartmentName;
          worksheet.Cells[i + 2, 4].Value = contracts[i].ContractTypeName;
          worksheet.Cells[i + 2, 5].Value = contracts[i].EndDays;
        }

        worksheet.Cells["A1:B1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = _localizer["lbl_ContractExpiryList"] + $"-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }


  }
}

using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace Exampler_ERP.Controllers.HR.Employeement
{
  public class ContractController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;

    public ContractController(AppDBContext appDBContext, IConfiguration configuration, Utils utils)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
    }

    public async Task<IActionResult> Index()
    {
      var contracts = await _appDBContext.HR_Contracts
          .Where(c => c.DeleteYNID != 1)
          .Include(c => c.Employee)
          .ToListAsync();

      return View("~/Views/HR/Employeement/Contract/Contract.cshtml", contracts);
    }

    public async Task<IActionResult> Contract()
    {
      var contracts = await _appDBContext.HR_Contracts.ToListAsync();
      return Ok(contracts);
    }

    public async Task<IActionResult> Edit(int id)
    {
      var contract = await _appDBContext.HR_Contracts.FindAsync(id);
      if (contract == null)
      {
        return NotFound();
      }

      ViewBag.EmployeesList = await _utils.GetEmployee();
      ViewBag.SalaryTypesList = await _utils.GetSalaryOptions();
      ViewBag.ContractTypesList = await _utils.GetContractTypes();

      return PartialView("~/Views/HR/Employeement/Contract/EditContract.cshtml", contract);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(HR_Contract contract)
    {
      if (ModelState.IsValid)
      {
        if (contract.ContractID == 2)
        {
          contract.EndDate = null;
        }
        _appDBContext.Update(contract);
        await _appDBContext.SaveChangesAsync();
        return Json(new { success = true });
      }
      return PartialView("~/Views/HR/Employeement/Contract/EditContract.cshtml", contract);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.EmployeesList = await _utils.GetEmployee();
      ViewBag.SalaryTypesList = await _utils.GetSalaryOptions();
      ViewBag.ContractTypesList = await _utils.GetContractTypes();

      return PartialView("~/Views/HR/Employeement/Contract/AddContract.cshtml", new HR_Contract());
    }

    [HttpPost]
    public async Task<IActionResult> Create(HR_Contract contract)
    {
      if (ModelState.IsValid)
      {
        contract.DeleteYNID = 0;
        contract.ActiveID = 0;
        if(contract.ContractID == 2)
        {
          contract.EndDate = null;
        }
        _appDBContext.HR_Contracts.Add(contract);
        await _appDBContext.SaveChangesAsync();
        return Json(new { success = true });
      }

      return PartialView("~/Views/HR/Employeement/Contract/AddContract.cshtml", contract);
    }

    public async Task<IActionResult> Delete(int id)
    {
      var contract = await _appDBContext.HR_Contracts.FindAsync(id);
      if (contract == null)
      {
        return NotFound();
      }

      contract.ActiveID = 0;
      contract.DeleteYNID = 1;

      _appDBContext.HR_Contracts.Update(contract);
      await _appDBContext.SaveChangesAsync();

      return Json(new { success = true });
    }

    public async Task<IActionResult> Print()
    {
      var contracts = await _appDBContext.HR_Contracts
          .Where(c => c.DeleteYNID != 1)
          .Include(c => c.Employee)
          .ToListAsync();

      return View("~/Views/HR/Employeement/Contract/PrintContracts.cshtml", contracts);
    }

    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var contracts = await _appDBContext.HR_Contracts
          .Where(c => c.DeleteYNID != 1)
          .Include(c => c.Employee)
          .ToListAsync();

      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add("Contracts");

        worksheet.Cells["A1"].Value = "Contract ID";
        worksheet.Cells["B1"].Value = "Employee Name";
        worksheet.Cells["C1"].Value = "Issue Date";
        worksheet.Cells["D1"].Value = "Salary Type";
        worksheet.Cells["E1"].Value = "Contract Type";
        worksheet.Cells["F1"].Value = "Vacation Days";
        worksheet.Cells["G1"].Value = "Daily Hours";
        worksheet.Cells["H1"].Value = "Daily Minutes";
        worksheet.Cells["I1"].Value = "Final Approval ID";
        worksheet.Cells["J1"].Value = "Approval Process ID";

        for (int i = 0; i < contracts.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = contracts[i].ContractID;
          worksheet.Cells[i + 2, 2].Value = contracts[i].EmployeeID; // assuming you want EmployeeID or you could use Employee.Name
          worksheet.Cells[i + 2, 3].Value = contracts[i].IssueDate.ToString("dd-MMM-yyyy");
          worksheet.Cells[i + 2, 4].Value = contracts[i].SalaryTypeID;
          worksheet.Cells[i + 2, 5].Value = contracts[i].ContractType;
          worksheet.Cells[i + 2, 6].Value = contracts[i].VacationDays;
          worksheet.Cells[i + 2, 7].Value = contracts[i].DHours;
          worksheet.Cells[i + 2, 8].Value = contracts[i].DMinutes;
          worksheet.Cells[i + 2, 9].Value = contracts[i].FinalApprovalID;
          worksheet.Cells[i + 2, 10].Value = contracts[i].ApprovalProcessID;
        }

        worksheet.Cells["A1:J1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = $"Contracts-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
  }
}

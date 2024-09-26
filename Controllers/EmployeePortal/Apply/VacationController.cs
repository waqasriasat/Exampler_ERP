using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace Exampler_ERP.Controllers.EmployeePortal.Apply
{
  public class VacationController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    public VacationController(AppDBContext appDBContext, IConfiguration configuration, Utils utils)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
    }
    public async Task<IActionResult> Index()
    {
      var employeeID = HttpContext.Session.GetInt32("EmployeeID");

      var vacations = await _appDBContext.HR_Vacations
                                   .Where(v => employeeID != null && v.EmployeeID == employeeID)
                                   .OrderByDescending(v => v.VacationID)
                                   .Include(c => c.Employee)
                                    .Include(c => c.Settings_VacationType)
                                   .ToListAsync();

      return View("~/Views/EmployeePortal/Apply/Vacation/Vacation.cshtml", vacations);
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
      ViewBag.VacationTypeList = await _utils.GetVacationTypes();
 
      return PartialView("~/Views/EmployeePortal/Apply/Vacation/AddVacation.cshtml", new HR_Vacation());
    }

    [HttpPost]
    public async Task<IActionResult> Create(HR_Vacation vacation)
    {
      if (ModelState.IsValid)
      {
        vacation.Date = DateTime.Now;
        vacation.TotalDays = (int)(vacation.EndDate - vacation.StartDate).TotalDays + 1;

        _appDBContext.HR_Vacations.Add(vacation);
        await _appDBContext.SaveChangesAsync();

        var vacationId = vacation.VacationID;
        if (vacationId > 0)
        {
          var processCount = await _appDBContext.CR_ProcessTypeApprovalSetups
                             .Where(pta => pta.ProcessTypeID > 0 && pta.ProcessTypeID == 11)
                             .CountAsync();

          if (processCount > 0)
          {
            var newProcessTypeApproval = new CR_ProcessTypeApproval
            {
              ProcessTypeID = 11,
              Notes = vacation.Reason,
              Date = DateTime.Now,
              EmployeeID = vacation.EmployeeID,
              UserID = 1, // Using session value
              TransactionID = vacation.VacationID
            };

            _appDBContext.CR_ProcessTypeApprovals.Add(newProcessTypeApproval);
            await _appDBContext.SaveChangesAsync();

            var nextApprovalSetup = await _appDBContext.CR_ProcessTypeApprovalSetups
                                     .Where(pta => pta.ProcessTypeID == 11 && pta.Rank == 1)
                                     .FirstOrDefaultAsync();

            if (nextApprovalSetup != null)
            {
              var newProcessTypeApprovalDetail = new CR_ProcessTypeApprovalDetail
              {
                ApprovalProcessID = newProcessTypeApproval.ApprovalProcessID,
                Date = DateTime.Now,
                RoleID = nextApprovalSetup.RoleID,
                AppID = 0,
                AppUserID = 0,
                Notes = null,
                Rank = nextApprovalSetup.Rank
              };

              _appDBContext.CR_ProcessTypeApprovalDetails.Add(newProcessTypeApprovalDetail);
              await _appDBContext.SaveChangesAsync();
              return Json(new { success = true });
            }
            else
            {
              return Json(new { success = false, message = "Next approval setup not found." });
            }
          }
          else
          {
            vacation.FinalApprovalID = 1;
            vacation.ApprovalProcessID = 0;
            _appDBContext.HR_Vacations.Update(vacation);
            await _appDBContext.SaveChangesAsync();
            return Json(new { success = true });
          }
        }

        return PartialView("~/Views/EmployeePortal/Apply/Vacation/AddVacation.cshtml", vacation);
      }

      return PartialView("~/Views/EmployeePortal/Apply/Vacation/AddVacation.cshtml", vacation);
    }


    public async Task<IActionResult> Delete(int id)
    {
      var contract = await _appDBContext.HR_Contracts.FindAsync(id);
      if (contract == null)
      {
        return NotFound();
      }

      contract.ActiveYNID = 0;
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
          .Include(c => c.Settings_ContractType)
          .ToListAsync();
      var SalaryTypesList = await _utils.GetSalaryOptions();
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
          worksheet.Cells[i + 2, 2].Value = contracts[i].Employee?.FirstName + ' ' + contracts[i].Employee?.FatherName + ' ' + contracts[i].Employee?.FamilyName;
          worksheet.Cells[i + 2, 3].Value = contracts[i].IssueDate.ToString("dd-MMM-yyyy");
          worksheet.Cells[i + 2, 4].Value = contracts[i].SalaryTypeID == 0 || contracts[i]?.SalaryTypeID == null
          ? ""
          : SalaryTypesList.FirstOrDefault(g => g.Value == contracts[i].SalaryTypeID.ToString())?.Text;
          worksheet.Cells[i + 2, 5].Value = contracts[i].ContractTypeID;
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

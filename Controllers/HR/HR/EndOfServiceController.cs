using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace Exampler_ERP.Controllers.HR.HR
{
  public class EndOfServiceController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;

    public EndOfServiceController(AppDBContext appDBContext, IConfiguration configuration, Utils utils)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
    }
    public async Task<IActionResult> Index()
    {
      var EndOfService = await _appDBContext.HR_EndOfServices
        .Where(b => b.DeleteYNID != 1)
        .Include(d => d.Employee)
        .ToListAsync();
      return View("~/Views/HR/HR/EndOfService/EndOfService.cshtml", EndOfService);
    }
    [HttpGet]
    public async Task<IActionResult> GetEmployeeDetails(int employeeID)
    {
      // Fetch employee details along with contract and contract type information
      var employeeDetails = await _appDBContext.HR_Contracts
          .Where(e => e.EmployeeID == employeeID)
          .Select(e => new
          {
            HireDate = e.Employee.HireDate.HasValue
                  ? e.Employee.HireDate.Value.ToString("MM/dd/yyyy")
                  : null,
            ContractTypeName = e.Settings_ContractType != null
                  ? e.Settings_ContractType.ContractTypeName
                  : "Unknown"
          })
          .FirstOrDefaultAsync();

      // Fetch basic salary detail for the employee
      var basicSalaryDetail = await _appDBContext.HR_SalaryDetails
          .Include(sd => sd.Salary)
          .Include(sd => sd.SalaryType)
          .Where(sd => sd.Salary.EmployeeID == employeeID && sd.SalaryType.SalaryTypeName == "Basic Salary")
          .Select(sd => new
          {
            SalaryAmount = sd.SalaryAmount ?? 0
          })
          .FirstOrDefaultAsync();

      if (employeeDetails == null)
      {
        return NotFound(); // Return 404 if no employee details found
      }

      // Combine employeeDetails and basicSalaryDetail in a single response
      var result = new
      {
        HireDate = employeeDetails.HireDate,
        ContractTypeName = employeeDetails.ContractTypeName,
        BasicSalary = basicSalaryDetail?.SalaryAmount ?? 0 // Handle null case for basic salary
      };

      return Json(result); // Return the combined result as JSON
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.EmployeesList = await _utils.GetEmployee();
      ViewBag.EndOfServiceReasonTypesList = await _utils.GetEndOfServiceReasonTypes();
      return PartialView("~/Views/HR/HR/EndOfService/AddEndOfService.cshtml", new HR_EndOfService());
    }
    [HttpPost]
    public async Task<IActionResult> Create(HR_EndOfService endofservice)
    {
      if (ModelState.IsValid)
      {
        endofservice.DeleteYNID = 0;
       
        _appDBContext.HR_EndOfServices.Add(endofservice);
        await _appDBContext.SaveChangesAsync();
        return Json(new { success = true });
      }

      return PartialView("~/Views/HR/HR/EndOfService/AddEndOfService.cshtml", endofservice);
    }
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
      var endOfService = await _appDBContext.HR_EndOfServices
          .Include(e => e.Employee)
          .FirstOrDefaultAsync(e => e.EndOfServiceID == id);

      if (endOfService == null)
      {
        return NotFound();
      }

      ViewBag.EmployeesList = await _utils.GetEmployee();
      ViewBag.EndOfServiceReasonTypesList = await _utils.GetEndOfServiceReasonTypes();
      return PartialView("~/Views/HR/HR/EndOfService/EditEndOfService.cshtml", endOfService);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(HR_EndOfService endofservice)
    {
      if (ModelState.IsValid)
      {
        _appDBContext.Update(endofservice);
        await _appDBContext.SaveChangesAsync();
        return Json(new { success = true });
     }

      ViewBag.EmployeesList = await _utils.GetEmployee();
      ViewBag.EndOfServiceReasonTypesList = await _utils.GetEndOfServiceReasonTypes();
      return PartialView("~/Views/HR/HR/EndOfService/EditEndOfService.cshtml", endofservice);
    }
    public async Task<IActionResult> Delete(int id)
    {
      var endofservice = await _appDBContext.HR_EndOfServices.FindAsync(id);
      if (endofservice == null)
      {
        return NotFound();
      }

      endofservice.DeleteYNID = 1;

      _appDBContext.HR_EndOfServices.Update(endofservice);
      await _appDBContext.SaveChangesAsync();

      return Json(new { success = true });
    }
    public async Task<IActionResult> Print()
    {
      var EndOfService = await _appDBContext.HR_EndOfServices
        .Where(b => b.DeleteYNID != 1)
        .Include(d => d.Employee)
        .Include(c => c.Settings_EndOfSerivceReasonType)
        .ToListAsync();
      return View("~/Views/HR/HR/EndOfService/PrintEndOfService.cshtml", EndOfService);
    }

    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var EndOfService = await _appDBContext.HR_EndOfServices
          .Where(c => c.DeleteYNID != 1)
          .Include(c => c.Employee)
          .Include(c => c.Settings_EndOfSerivceReasonType)
          .ToListAsync();

      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add("EndOfServices");

        worksheet.Cells["A1"].Value = "EndOfService ID";
        worksheet.Cells["B1"].Value = "Employee Name";
        worksheet.Cells["C1"].Value = "Reason Type";
        worksheet.Cells["D1"].Value = "Total Salary";
        worksheet.Cells["E1"].Value = "Total Day Of Absent";
        worksheet.Cells["F1"].Value = "Date Of Commencement";
        worksheet.Cells["G1"].Value = "Date Of Completion Of Work";
        worksheet.Cells["H1"].Value = "Number of Years";
        worksheet.Cells["I1"].Value = "Number of Months";
        worksheet.Cells["J1"].Value = "Number of Days";
        worksheet.Cells["K1"].Value = "Total Days";
        worksheet.Cells["L1"].Value = "End of Service Benefit";
        worksheet.Cells["M1"].Value = "Balance Of The Annual Leave";
        worksheet.Cells["N1"].Value = "Amount Dues For The Leave";
        worksheet.Cells["O1"].Value = "Total End Of Service Dues";

        for (int i = 0; i < EndOfService.Count; i++)
        {
          int employeeID = EndOfService[i].EmployeeID;

          // Query for fetching the basic salary
          var basicSalaryDetail = await _appDBContext.HR_SalaryDetails
              .Include(sd => sd.Salary)
              .Include(sd => sd.SalaryType)
              .Where(sd => sd.Salary.EmployeeID == employeeID && sd.SalaryType.SalaryTypeName == "Basic Salary")
              .Select(sd => new
              {
                SalaryAmount = sd.SalaryAmount ?? 0 // Default to 0 if SalaryAmount is null
              })
              .FirstOrDefaultAsync();

          // Set Excel worksheet values
          worksheet.Cells[i + 2, 1].Value = EndOfService[i].EndOfServiceID;
          worksheet.Cells[i + 2, 2].Value = EndOfService[i].Employee?.FirstName + ' ' + EndOfService[i].Employee?.FatherName + ' ' + EndOfService[i].Employee?.FamilyName;
          worksheet.Cells[i + 2, 3].Value = EndOfService[i].Settings_EndOfSerivceReasonType?.EndOfServiceReasonTypeName;

          // Set the basic salary amount from the query result
          worksheet.Cells[i + 2, 4].Value = basicSalaryDetail?.SalaryAmount ?? 0;

          worksheet.Cells[i + 2, 5].Value = ""; // Empty value for this cell
          worksheet.Cells[i + 2, 6].Value = EndOfService[i].Employee?.HireDate?.ToString("dd-MMM-yyyy");
          worksheet.Cells[i + 2, 7].Value = EndOfService[i].DateOfCompletionOfWork.ToString("dd-MMM-yyyy");
          worksheet.Cells[i + 2, 8].Value = EndOfService[i].NumberofYears;
          worksheet.Cells[i + 2, 9].Value = EndOfService[i].NumberofMonths;
          worksheet.Cells[i + 2, 10].Value = EndOfService[i].NumberofDays;
          worksheet.Cells[i + 2, 11].Value = EndOfService[i].TotalDays;
          worksheet.Cells[i + 2, 12].Value = EndOfService[i].EndofServiceBenefit;
          worksheet.Cells[i + 2, 13].Value = EndOfService[i].BalanceOfTheAnnualLeave;
          worksheet.Cells[i + 2, 14].Value = EndOfService[i].AmountDueForTheLeave;
          worksheet.Cells[i + 2, 15].Value = EndOfService[i].TotalEndOfServiceDues;
        }


        worksheet.Cells["A1:J1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = $"EndOfService-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
  }
}

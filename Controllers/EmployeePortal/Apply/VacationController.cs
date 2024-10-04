using Exampler_ERP.Models;
using Exampler_ERP.Models.Temp;
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
                                   .Where(v => employeeID != null && v.EmployeeID == employeeID && v.DeleteYNID != 1)
                                   .OrderByDescending(v => v.VacationID)
                                   .Include(c => c.Employee)
                                    .Include(c => c.Settings_VacationType)
                                   .ToListAsync();

      return View("~/Views/EmployeePortal/Apply/Vacation/Vacation.cshtml", vacations);
    }
    public async Task<IActionResult> Edit(int id)
    {
      ViewBag.VacationTypeList = await _utils.GetVacationTypes();
      var vacations = await _appDBContext.HR_Vacations
      .Where(v => v.VacationID == id)
                                   .Include(c => c.Employee)
                                    .Include(c => c.Settings_VacationType)
                                   .FirstOrDefaultAsync();

      return PartialView("~/Views/EmployeePortal/Apply/Vacation/EditVacation.cshtml", vacations);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(HR_Vacation vacation)
    {
      if (ModelState.IsValid)
      {
        // Calculate total days between StartDate and EndDate
        vacation.TotalDays = (int)(vacation.EndDate - vacation.StartDate).TotalDays + 1;

        _appDBContext.Update(vacation);
        await _appDBContext.SaveChangesAsync();
        return Json(new { success = true });
      }
      return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
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

        // Calculate total days between StartDate and EndDate
        vacation.TotalDays = (int)(vacation.EndDate - vacation.StartDate).TotalDays + 1;

        // Add the vacation entry to the database
        _appDBContext.Add(vacation);
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
                RoleID = nextApprovalSetup.RoleTypeID,
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
              return Json(new { success = true });
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

        return Json(new { success = true });
      }

      // If the model state is invalid, return a failure response with validation errors
      return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
    }


    public async Task<IActionResult> Delete(int id)
    {
      var vacations = await _appDBContext.HR_Vacations.FindAsync(id);
      if (vacations == null)
      {
        return NotFound();
      }

      vacations.DeleteYNID = 1;

      _appDBContext.HR_Vacations.Update(vacations);
      await _appDBContext.SaveChangesAsync();

      return Json(new { success = true });
    }

    public async Task<IActionResult> Print()
    {
      var employeeID = HttpContext.Session.GetInt32("EmployeeID");

      var vacations = await _appDBContext.HR_Vacations
                                   .Where(v => employeeID != null && v.EmployeeID == employeeID && v.DeleteYNID != 1)
                                   .OrderByDescending(v => v.VacationID)
                                   .Include(c => c.Employee)
                                    .Include(c => c.Settings_VacationType)
                                   .ToListAsync();

      return View("~/Views/EmployeePortal/Apply/Vacation/PrintVacation.cshtml", vacations);
    }

    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var employeeID = HttpContext.Session.GetInt32("EmployeeID");

      var vacations = await _appDBContext.HR_Vacations
                                   .Where(v => employeeID != null && v.EmployeeID == employeeID && v.DeleteYNID !=1)
                                   .OrderByDescending(v => v.VacationID)
                                   .Include(c => c.Employee)
                                    .Include(c => c.Settings_VacationType)
                                   .ToListAsync();

    
      var vacationTypesList = await _utils.GetVacationTypes();
      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add("Vacations");

        worksheet.Cells["A1"].Value = "Vacation ID";
        worksheet.Cells["B1"].Value = "Employee Name";
        worksheet.Cells["C1"].Value = "Vacation Type";
        worksheet.Cells["D1"].Value = "Apply Date";
        worksheet.Cells["E1"].Value = "Start Date";
        worksheet.Cells["F1"].Value = "End Date";
        worksheet.Cells["G1"].Value = "Total Days";
       
        for (int i = 0; i < vacations.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = vacations[i].VacationID;
          worksheet.Cells[i + 2, 2].Value = vacations[i].Employee?.FirstName + ' ' + vacations[i].Employee?.FatherName + ' ' + vacations[i].Employee?.FamilyName;
          worksheet.Cells[i + 2, 3].Value = vacations[i].VacationTypeID == 0 || vacations[i]?.VacationTypeID == null
          ? ""
          : vacationTypesList.FirstOrDefault(g => g.Value == vacations[i].VacationTypeID.ToString())?.Text;
          worksheet.Cells[i + 2, 4].Value = vacations[i].Date.ToString("dd-MMM-yyyy");
          worksheet.Cells[i + 2, 5].Value = vacations[i].StartDate.ToString("dd-MMM-yyyy");
          worksheet.Cells[i + 2, 6].Value = vacations[i].EndDate.ToString("dd-MMM-yyyy");
          worksheet.Cells[i + 2, 7].Value = vacations[i].TotalDays;
          
        }

        worksheet.Cells["B1:G1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = $"vacations-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
  }
}

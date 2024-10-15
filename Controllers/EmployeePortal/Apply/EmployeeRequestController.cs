using Exampler_ERP.Models;
using Exampler_ERP.Models.Temp;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace Exampler_ERP.Controllers.EmployeePortal.Apply
{
  public class EmployeeRequestTypeController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    public EmployeeRequestTypeController(AppDBContext appDBContext, IConfiguration configuration, Utils utils)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
    }
    public async Task<IActionResult> Index()
    {
      var employeeID = HttpContext.Session.GetInt32("EmployeeID");

      var EmployeeRequestTypes = await _appDBContext.HR_EmployeeRequestTypeApprovals
                                   .Where(v => employeeID != null && v.EmployeeID == employeeID && v.DeleteYNID != 1)
                                   .OrderByDescending(v => v.EmployeeRequestTypeID)
                                   .Include(c => c.Employee)
                                    .Include(c => c.EmployeeRequestType)
                                   .ToListAsync();

      return View("~/Views/EmployeePortal/Apply/EmployeeRequestType/EmployeeRequestType.cshtml", EmployeeRequestTypes);
    }
    public async Task<IActionResult> Edit(int id)
    {
      ViewBag.EmployeeRequestTypeList = await _utils.GetEmployeeRequestTypes();
      var EmployeeRequestTypes = await _appDBContext.HR_EmployeeRequestTypeApprovals
      .Where(v => v.EmployeeRequestTypeID == id)
                                   .Include(c => c.Employee)
                                    .Include(c => c.EmployeeRequestType)
                                   .FirstOrDefaultAsync();

      return PartialView("~/Views/EmployeePortal/Apply/EmployeeRequestType/EditEmployeeRequestType.cshtml", EmployeeRequestTypes);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(HR_EmployeeRequestTypeApproval EmployeeRequestType)
    {
      if (ModelState.IsValid)
      {
     
        _appDBContext.Update(EmployeeRequestType);
        await _appDBContext.SaveChangesAsync();
        return Json(new { success = true });
      }
      return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.EmployeeRequestTypeList = await _utils.GetEmployeeRequestTypes();

      return PartialView("~/Views/EmployeePortal/Apply/EmployeeRequestType/AddEmployeeRequestType.cshtml", new HR_EmployeeRequestTypeApproval());
    }

    [HttpPost]
    public async Task<IActionResult> Create(HR_EmployeeRequestTypeApproval EmployeeRequestType)
    {
      if (ModelState.IsValid)
      {
        EmployeeRequestType.Date = DateTime.Now;

    
        // Add the EmployeeRequestType entry to the database
        _appDBContext.Add(EmployeeRequestType);
        await _appDBContext.SaveChangesAsync();

        var EmployeeRequestTypeId = EmployeeRequestType.EmployeeRequestTypeID;
        if (EmployeeRequestTypeId > 0)
        {
          var processCount = await _appDBContext.CR_ProcessTypeApprovalSetups
                             .Where(pta => pta.ProcessTypeID > 0 && pta.ProcessTypeID == 14)
                             .CountAsync();

          if (processCount > 0)
          {
            var newProcessTypeApproval = new CR_ProcessTypeApproval
            {
              ProcessTypeID = 14,
              Notes = EmployeeRequestType.Notes,
              Date = DateTime.Now,
              EmployeeID = EmployeeRequestType.EmployeeID,
              UserID = 1, // Using session value
              TransactionID = EmployeeRequestType.EmployeeRequestTypeID
            };

            _appDBContext.CR_ProcessTypeApprovals.Add(newProcessTypeApproval);
            await _appDBContext.SaveChangesAsync();

            var nextApprovalSetup = await _appDBContext.CR_ProcessTypeApprovalSetups
                                     .Where(pta => pta.ProcessTypeID == 14 && pta.Rank == 1)
                                     .FirstOrDefaultAsync();

            if (nextApprovalSetup != null)
            {
              var newProcessTypeApprovalDetail = new CR_ProcessTypeApprovalDetail
              {
                ProcessTypeApprovalID = newProcessTypeApproval.ProcessTypeApprovalID,
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
            EmployeeRequestType.FinalApprovalID = 1;
            EmployeeRequestType.ProcessTypeApprovalID = 0;
            _appDBContext.HR_EmployeeRequestTypeApprovals.Update(EmployeeRequestType);
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
      var EmployeeRequestTypes = await _appDBContext.HR_EmployeeRequestTypeApprovals.FindAsync(id);
      if (EmployeeRequestTypes == null)
      {
        return NotFound();
      }

      EmployeeRequestTypes.DeleteYNID = 1;

      _appDBContext.HR_EmployeeRequestTypeApprovals.Update(EmployeeRequestTypes);
      await _appDBContext.SaveChangesAsync();

      return Json(new { success = true });
    }

    public async Task<IActionResult> Print()
    {
      var employeeID = HttpContext.Session.GetInt32("EmployeeID");

      var EmployeeRequestTypes = await _appDBContext.HR_EmployeeRequestTypeApprovals
                                   .Where(v => employeeID != null && v.EmployeeID == employeeID && v.DeleteYNID != 1)
                                   .OrderByDescending(v => v.EmployeeRequestTypeID)
                                   .Include(c => c.Employee)
                                    .Include(c => c.EmployeeRequestType)
                                   .ToListAsync();

      return View("~/Views/EmployeePortal/Apply/EmployeeRequestType/PrintEmployeeRequestType.cshtml", EmployeeRequestTypes);
    }

    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var employeeID = HttpContext.Session.GetInt32("EmployeeID");

      var EmployeeRequestTypes = await _appDBContext.HR_EmployeeRequestTypeApprovals
                                   .Where(v => employeeID != null && v.EmployeeID == employeeID && v.DeleteYNID != 1)
                                   .OrderByDescending(v => v.EmployeeRequestTypeID)
                                   .Include(c => c.Employee)
                                    .Include(c => c.EmployeeRequestType)
                                   .ToListAsync();


      var EmployeeRequestTypesList = await _utils.GetEmployeeRequestTypes();
      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add("EmployeeRequestTypes");

        worksheet.Cells["A1"].Value = "EmployeeRequestType ID";
        worksheet.Cells["B1"].Value = "Employee Name";
        worksheet.Cells["C1"].Value = "EmployeeRequestType Type";
        worksheet.Cells["D1"].Value = "Apply Date";
        worksheet.Cells["E1"].Value = "Notes";
       

        for (int i = 0; i < EmployeeRequestTypes.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = EmployeeRequestTypes[i].EmployeeRequestTypeID;
          worksheet.Cells[i + 2, 2].Value = EmployeeRequestTypes[i].Employee?.FirstName + ' ' + EmployeeRequestTypes[i].Employee?.FatherName + ' ' + EmployeeRequestTypes[i].Employee?.FamilyName;
          worksheet.Cells[i + 2, 3].Value = EmployeeRequestTypes[i].EmployeeRequestTypeID == 0 || EmployeeRequestTypes[i]?.EmployeeRequestTypeID == null
          ? ""
          : EmployeeRequestTypesList.FirstOrDefault(g => g.Value == EmployeeRequestTypes[i].EmployeeRequestTypeID.ToString())?.Text;
          worksheet.Cells[i + 2, 4].Value = EmployeeRequestTypes[i].Date.ToString("dd-MMM-yyyy");
          worksheet.Cells[i + 2, 5].Value = EmployeeRequestTypes[i].Notes;
          

        }

        worksheet.Cells["B1:G1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = $"EmployeeRequestTypes-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
  }
}

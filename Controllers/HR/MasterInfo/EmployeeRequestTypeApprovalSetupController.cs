using Exampler_ERP.Models.Temp;
using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Localization;

namespace Exampler_ERP.Controllers.HR.MasterInfo
{
  public class EmployeeRequestTypeApprovalSetupController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IStringLocalizer<EmployeeRequestTypeApprovalSetupController> _localizer;
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmployeeRequestTypeApprovalSetupController> _logger;
    private readonly Utils _utils;
    private readonly IHubContext<NotificationHub> _hubContext;

    public EmployeeRequestTypeApprovalSetupController(AppDBContext appDBContext, IConfiguration configuration, ILogger<EmployeeRequestTypeApprovalSetupController> logger, Utils utils, IHubContext<NotificationHub> hubContext, IStringLocalizer<EmployeeRequestTypeApprovalSetupController> localizer)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _logger = logger;
      _utils = utils;
      _hubContext = hubContext;
      _localizer = localizer;

    }
    public async Task<IActionResult> Index(string searchEmployeeRequestTypeName)
    {
      // Query for EmployeeRequest Types with an optional search filter
      var EmployeeRequestTypesQuery = _appDBContext.Settings_EmployeeRequestTypes.AsQueryable();

      if (!string.IsNullOrEmpty(searchEmployeeRequestTypeName))
      {
        EmployeeRequestTypesQuery = EmployeeRequestTypesQuery
            .Where(pt => pt.EmployeeRequestTypeName.Contains(searchEmployeeRequestTypeName));
      }

      var EmployeeRequestTypes = await EmployeeRequestTypesQuery.ToListAsync();
      var EmployeeRequestTypeApprovalSetup = await _appDBContext.HR_EmployeeRequestTypeApprovalSetups.ToListAsync();

      // Create ViewModel based on query results
      var viewModel = new EmployeeRequestTypeApprovalSetupListViewModel
      {
        EmployeeRequestTypesWithRankCount = EmployeeRequestTypes.Select(pt => new EmployeeRequestTypeWithRankCountViewModel
        {
          EmployeeRequestTypeID = pt.EmployeeRequestTypeID,
          EmployeeRequestTypeName = pt.EmployeeRequestTypeName,
          RankCount = EmployeeRequestTypeApprovalSetup.Count(ptf => ptf.EmployeeRequestTypeID == pt.EmployeeRequestTypeID)
        }).ToList()
      };

      if (!string.IsNullOrEmpty(searchEmployeeRequestTypeName) && viewModel.EmployeeRequestTypesWithRankCount.Count == 0)
      {
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "No Employee Request Type found with the name '" + searchEmployeeRequestTypeName + "'. Please check the name and try again.");
      }

      return View("~/Views/HR/MasterInfo/EmployeeRequestTypeApprovalSetup/EmployeeRequestTypeApprovalSetup.cshtml", viewModel);
    }


    public async Task<IActionResult> EmployeeRequestTypeApprovalSetup()
    {
      var EmployeeRequestTypeApprovalSetups = await _appDBContext.HR_EmployeeRequestTypeApprovalSetups.ToListAsync();
      return Ok(EmployeeRequestTypeApprovalSetups);
    }
    public async Task<IActionResult> Edit(int id)
    {
      var EmployeeRequestTypeApprovalSetups = await _appDBContext.HR_EmployeeRequestTypeApprovalSetups
       .Where(pt => pt.EmployeeRequestTypeID == id)
       .Include(pt => pt.EmployeeRequestType)
       .Include(pt => pt.RoleType)
       .ToListAsync();

      if (EmployeeRequestTypeApprovalSetups == null || !EmployeeRequestTypeApprovalSetups.Any())
      {
        var EmployeeRequestType = await _appDBContext.Settings_EmployeeRequestTypes
            .Where(p => p.EmployeeRequestTypeID == id)
            .FirstOrDefaultAsync();

        if (EmployeeRequestType != null)
        {
          EmployeeRequestTypeApprovalSetups = new List<HR_EmployeeRequestTypeApprovalSetup>
            {
                new HR_EmployeeRequestTypeApprovalSetup
                {
                    EmployeeRequestTypeID = EmployeeRequestType.EmployeeRequestTypeID,
                    EmployeeRequestType = EmployeeRequestType
                }
            };
        }
        else
        {
          return NotFound(); // Or handle accordingly if EmployeeRequestType is not found
        }
      }

      ViewBag.RoleList = await _appDBContext.Settings_RoleTypes
          .Select(r => new { Value = r.RoleTypeID, Text = r.RoleTypeName })
          .ToListAsync();

      ViewBag.EmployeeRequestTypes = await _appDBContext.Settings_EmployeeRequestTypes.ToListAsync();

      return PartialView("~/Views/HR/MasterInfo/EmployeeRequestTypeApprovalSetup/EditEmployeeRequestTypeApprovalSetup.cshtml", EmployeeRequestTypeApprovalSetups);
    }



    [HttpPost]
    public async Task<IActionResult> Edit(List<HR_EmployeeRequestTypeApprovalSetup> EmployeeRequestTypeApprovalSetups)
    {
      foreach (var setup in EmployeeRequestTypeApprovalSetups)
      {
        _logger.LogInformation("Received Setup: ID={ID}, Rank={Rank}, RoleID={RoleID}", setup.EmployeeRequestTypeApprovalSetupID, setup.Rank, setup.RoleTypeID);
      }

      if (EmployeeRequestTypeApprovalSetups == null || EmployeeRequestTypeApprovalSetups.Count == 0)
      {
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "No data received.");
        _logger.LogWarning("No data received for edit.");
        return Json(new { success = false });
      }

      if (ModelState.IsValid)
      {
        try
        {
          var EmployeeRequestTypeId = EmployeeRequestTypeApprovalSetups.FirstOrDefault()?.EmployeeRequestTypeID;
          if (EmployeeRequestTypeId == null)
          {
            await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "Invalid EmployeeRequestTypeID.");
            return BadRequest("Invalid EmployeeRequestTypeID.");
          }

          var existingRecords = await _appDBContext.HR_EmployeeRequestTypeApprovalSetups
                                        .Where(p => p.EmployeeRequestTypeID == EmployeeRequestTypeId)
                                        .ToListAsync();

          _appDBContext.HR_EmployeeRequestTypeApprovalSetups.RemoveRange(existingRecords);

          foreach (var setup in EmployeeRequestTypeApprovalSetups)
          {
            if (setup.RoleTypeID != 0 && setup.Rank != 0)
            {
              _appDBContext.HR_EmployeeRequestTypeApprovalSetups.Add(setup);
            }
          }

          await _appDBContext.SaveChangesAsync();
          await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Employee Request Type Approval Setup Update successfully.");
          return Json(new { success = true });
        }
        catch (Exception ex)
        {
          await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "An error occurred while updating the data.");
          _logger.LogError(ex, "Error updating EmployeeRequestTypeApprovalSetups");
          return Json(new { success = false });
        }
      }

      var errors = ModelState.Values.SelectMany(v => v.Errors);
      return PartialView("~/Views/HR/MasterInfo/EmployeeRequestTypeApprovalSetup/EditEmployeeRequestTypeApprovalSetup.cshtml", EmployeeRequestTypeApprovalSetups);
    }



    [HttpPost]
    public IActionResult Delete(int id)
    {
      var setups = _appDBContext.HR_EmployeeRequestTypeApprovalSetups
          .Where(x => x.EmployeeRequestTypeID == id)
          .ToList();

      if (setups == null || setups.Count == 0)
      {
        return Json(new { success = false, message = "Setup not found" });
      }

      _appDBContext.HR_EmployeeRequestTypeApprovalSetups.RemoveRange(setups);
      _appDBContext.SaveChanges();
      _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Employee Request Type Approval Setups deleted successfully.");
      return Json(new { success = true });
    }



    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var EmployeeRequestTypeApprovalSetups = await _appDBContext.HR_EmployeeRequestTypeApprovalSetups
      .Distinct()
      .Include(d => d.EmployeeRequestType) // Eagerly load the related Branch data
      .ToListAsync();


      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add(_localizer["lbl_EmployeeRequestTypeApprovalSetup"]);
        worksheet.Cells["A1"].Value = _localizer["lbl_EmployeeRequestTypeApprovalSetupID"];
        worksheet.Cells["B1"].Value = _localizer["lbl_EmployeeRequestTypeName"];
        worksheet.Cells["C1"].Value = _localizer["lbl_Rank"];
        worksheet.Cells["D1"].Value = _localizer["lbl_RoleType"];


        for (int i = 0; i < EmployeeRequestTypeApprovalSetups.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = EmployeeRequestTypeApprovalSetups[i].EmployeeRequestTypeApprovalSetupID;
          worksheet.Cells[i + 2, 2].Value = EmployeeRequestTypeApprovalSetups[i].EmployeeRequestType?.EmployeeRequestTypeName;
          worksheet.Cells[i + 2, 3].Value = EmployeeRequestTypeApprovalSetups[i].Rank;
          worksheet.Cells[i + 2, 4].Value = EmployeeRequestTypeApprovalSetups[i].RoleTypeID;
        }

        worksheet.Cells["A1:l1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = _localizer["lbl_EmployeeRequestTypeApprovalSetup"]+$"-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
    public async Task<IActionResult> Print()
    {
      var EmployeeRequestTypeApprovalSetups = await _appDBContext.HR_EmployeeRequestTypeApprovalSetups
      .Distinct()
      .Include(d => d.EmployeeRequestType) // Eagerly load the related Branch data
      .ToListAsync();
      return View("~/Views/HR/MasterInfo/EmployeeRequestTypeApprovalSetup/PrintEmployeeRequestTypeApprovalSetup.cshtml", EmployeeRequestTypeApprovalSetups);
    }

  }
}

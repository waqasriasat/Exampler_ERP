using Exampler_ERP.Models.Temp;
using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using Microsoft.EntityFrameworkCore;

namespace Exampler_ERP.Controllers.Setup
{
    public class AccessRightsByRoleController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AccessRightsByRoleController> _logger;
    private readonly Utils _utils;
    public AccessRightsByRoleController(AppDBContext appDBContext, IConfiguration configuration, ILogger<AccessRightsByRoleController> logger, Utils utils)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _logger = logger;
      _utils = utils;
    }
    public async Task<IActionResult> Index(string searchRoleName)
    {
      var roleTypeQuery = _appDBContext.Settings_RoleTypes.AsQueryable();

      if (!string.IsNullOrEmpty(searchRoleName))
      {
        roleTypeQuery = roleTypeQuery
            .Where(pt => pt.RoleTypeName.Contains(searchRoleName));
      }

      var roleTypes = await roleTypeQuery.ToListAsync();
      var AccessRightsByRoleSetup = await _appDBContext.CR_AccessRightsByRoles.ToListAsync();

      var viewModel = new AccessRightsByRoleListViewModel
      {
        AccessRightsWithRoleCount = roleTypes.Select(pt =>
        {
          var roleTypeId = "_" + pt.RoleTypeID;

          int totalAccessCount = AccessRightsByRoleSetup.Count(ptf =>
          {
            var property = ptf.GetType().GetProperty(roleTypeId);
            if (property != null)
            {
              var value = property.GetValue(ptf);
              return value is int && (int)value == 1;
            }
            return false;
          });

          return new AccessRightsWithRoleCountViewModel
          {
            RoleID = pt.RoleTypeID,
            RoleName = pt.RoleTypeName,
            TotalAccessCount = totalAccessCount
          };
        }).ToList()
      };


      if (!string.IsNullOrEmpty(searchRoleName) && viewModel.AccessRightsWithRoleCount.Count == 0)
      {
        TempData["ErrorMessage"] = "No Role found with the name '" + searchRoleName + "'. Please check the name and try again.";
      }

      return View("~/Views/Setup/AccessRightsByRole/AccessRightsByRole.cshtml", viewModel);
    }


    public async Task<IActionResult> ProcessTypeApprovalSetup()
    {
      var ProcessTypeApprovalSetups = await _appDBContext.CR_ProcessTypeApprovalSetups.ToListAsync();
      return Ok(ProcessTypeApprovalSetups);
    }
    public async Task<IActionResult> Edit(int id)
    {
      var processTypeApprovalSetups = await _appDBContext.CR_ProcessTypeApprovalSetups
       .Where(pt => pt.ProcessTypeID == id)
       .Include(pt => pt.ProcessType)
       .Include(pt => pt.RoleType)
       .ToListAsync();

      if (processTypeApprovalSetups == null || !processTypeApprovalSetups.Any())
      {
        var processType = await _appDBContext.Settings_ProcessTypes
            .Where(p => p.ProcessTypeID == id)
            .FirstOrDefaultAsync();

        if (processType != null)
        {
          processTypeApprovalSetups = new List<CR_ProcessTypeApprovalSetup>
            {
                new CR_ProcessTypeApprovalSetup
                {
                    ProcessTypeID = processType.ProcessTypeID,
                    ProcessType = processType
                }
            };
        }
        else
        {
          return NotFound(); // Or handle accordingly if ProcessType is not found
        }
      }

      ViewBag.RoleList = await _appDBContext.Settings_RoleTypes
          .Select(r => new { Value = r.RoleTypeID, Text = r.RoleTypeName })
          .ToListAsync();

      ViewBag.ProcessTypes = await _appDBContext.Settings_ProcessTypes.ToListAsync();
      return PartialView("~/Views/Setup/AccessRightsByRole/EditAccessRightsByRole.cshtml", processTypeApprovalSetups);
    }



    [HttpPost]
    public async Task<IActionResult> Edit(List<CR_ProcessTypeApprovalSetup> ProcessTypeApprovalSetups)
    {
      foreach (var setup in ProcessTypeApprovalSetups)
      {
        _logger.LogInformation("Received Setup: ID={ID}, Rank={Rank}, RoleID={RoleID}", setup.ProcessTypeApprovalSetupID, setup.Rank, setup.RoleTypeID);
      }

      if (ProcessTypeApprovalSetups == null || ProcessTypeApprovalSetups.Count == 0)
      {
        TempData["ErrorMessage"] = "No data received.";
        _logger.LogWarning("No data received for edit.");
        return Json(new { success = false, message = "No data received." });
      }

      if (ModelState.IsValid)
      {
        try
        {
          var processTypeId = ProcessTypeApprovalSetups.FirstOrDefault()?.ProcessTypeID;
          if (processTypeId == null)
          {
            TempData["ErrorMessage"] = "Invalid ProcessTypeID.";
            return BadRequest("Invalid ProcessTypeID.");
          }

          var existingRecords = await _appDBContext.CR_ProcessTypeApprovalSetups
                                        .Where(p => p.ProcessTypeID == processTypeId)
                                        .ToListAsync();

          _appDBContext.CR_ProcessTypeApprovalSetups.RemoveRange(existingRecords);

          foreach (var setup in ProcessTypeApprovalSetups)
          {
            if (setup.RoleTypeID != 0 && setup.Rank != 0)
            {
              _appDBContext.CR_ProcessTypeApprovalSetups.Add(setup);
            }
          }

          await _appDBContext.SaveChangesAsync();
          TempData["SuccessMessage"] = "Process Type Approval Setup Update successfully.";
          return Json(new { success = true });
        }
        catch (Exception ex)
        {
          TempData["ErrorMessage"] = "An error occurred while updating the data.";
          _logger.LogError(ex, "Error updating ProcessTypeApprovalSetups");
          return Json(new { success = false, message = "An error occurred while updating the data." });
        }
      }

      var errors = ModelState.Values.SelectMany(v => v.Errors);
      return PartialView("~/Views/HR/MasterInfo/ProcessTypeApprovalSetup/EditProcessTypeApprovalSetup.cshtml", ProcessTypeApprovalSetups);
    }



    [HttpPost]
    public IActionResult Delete(int id)
    {
      var setups = _appDBContext.CR_ProcessTypeApprovalSetups
        .Where(x => x.ProcessTypeID == id)
        .ToList();
      if (setups == null)
      {
        return Json(new { success = false, message = "Setup not found" });
      }

      _appDBContext.CR_ProcessTypeApprovalSetups.RemoveRange(setups);
      _appDBContext.SaveChanges();
      TempData["SuccessMessage"] = "Process Type Approval Setups deleted successfully.";
      return Json(new { success = true });
    }


    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var ProcessTypeApprovalSetups = await _appDBContext.CR_ProcessTypeApprovalSetups
      .Distinct()
      .Include(d => d.ProcessType) // Eagerly load the related Branch data
      .ToListAsync();


      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add("ProcessTypeApprovalSetups");
        worksheet.Cells["A1"].Value = "ProcessType SetupID";
        worksheet.Cells["B1"].Value = "ProcessType Name";
        worksheet.Cells["C1"].Value = "Rank";
        worksheet.Cells["D1"].Value = "Role";


        for (int i = 0; i < ProcessTypeApprovalSetups.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = ProcessTypeApprovalSetups[i].ProcessTypeApprovalSetupID;
          worksheet.Cells[i + 2, 2].Value = ProcessTypeApprovalSetups[i].ProcessType?.ProcessTypeName;
          worksheet.Cells[i + 2, 3].Value = ProcessTypeApprovalSetups[i].Rank;
          worksheet.Cells[i + 2, 4].Value = ProcessTypeApprovalSetups[i].RoleTypeID;
        }

        worksheet.Cells["A1:l1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = $"ProcessTypeApprovalSetups-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
    public async Task<IActionResult> Print()
    {
      var ProcessTypeApprovalSetups = await _appDBContext.CR_ProcessTypeApprovalSetups
      .Distinct()
      .Include(d => d.ProcessType) // Eagerly load the related Branch data
      .ToListAsync();
      return View("~/Views/HR/MasterInfo/ProcessTypeApprovalSetup/PrintProcessTypeApprovalSetups.cshtml", ProcessTypeApprovalSetups);
    }

  }
}

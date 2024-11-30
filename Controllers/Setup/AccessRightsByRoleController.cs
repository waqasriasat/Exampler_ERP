using Exampler_ERP.Models.Temp;
using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;


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

    public async Task<IActionResult> Edit(int id)
    {
      var roleTypeProperty = $"_{id}";

      ViewBag.RoleTypeProperty = roleTypeProperty;

      var accessRightsByRoles = await _appDBContext.CR_AccessRightsByRoles
       .Where(u => EF.Property<int>(u, roleTypeProperty) == 1)
       .ToListAsync();

      var role = await _appDBContext.Settings_RoleTypes
      .FirstOrDefaultAsync(u => u.RoleTypeID == id);

      if (role != null && !string.IsNullOrEmpty(role.RoleTypeName))
      {
        ViewBag.RoleName = role.RoleTypeName;
      }


      if (accessRightsByRoles == null || !accessRightsByRoles.Any())
      {
        var accessRightByRole_All = await _appDBContext.CR_AccessRightsByRoles.FirstOrDefaultAsync();

        if (accessRightByRole_All != null)
        {
          accessRightsByRoles = new List<CR_AccessRightsByRole>
            {
                new CR_AccessRightsByRole
                {
                    ActionSOR = accessRightByRole_All.ActionSOR,
                    ActionName = accessRightByRole_All.ActionName,
                    ActionType = accessRightByRole_All.ActionType,
                    AccessRightID = accessRightByRole_All.AccessRightID,
                    MenuID = accessRightByRole_All.MenuID,
                    ModuleID = accessRightByRole_All.ModuleID
                }
            };
        }
        else
        {
          return NotFound();
        }
      }

      ViewBag.AccessRoleList = await _appDBContext.CR_AccessRightsByRoles
         .Select(r => new SelectListItem
         {
           Value = r.ActionSOR.ToString(),
           Text = r.ActionName
         })
         .ToListAsync();

      return PartialView("~/Views/Setup/AccessRightsByRole/EditAccessRightsByRole.cshtml", accessRightsByRoles);
    }
    public async Task<IActionResult> GetRoleRow(string actionSOR)
    {
      // Fetch the first matching record based on actionSOR value
      var data = await _appDBContext.CR_AccessRightsByRoles
          .Where(u => u.ActionSOR.ToString() == actionSOR) // Ensure comparison works correctly
          .FirstOrDefaultAsync();

      // Check if data exists, if not return an error or an empty object
      if (data == null)
      {
        return Json(new { success = false, message = "No data found" });
      }

      // Return data as JSON
      return Json(new
      {
        success = true,
        accessRightID = data.AccessRightID,
        actionType = data.ActionType,
        moduleID = data.ModuleID,
        menuID = data.MenuID,
        actionName = data.ActionName
      });
    }



    [HttpPost]
    public async Task<IActionResult> Edit(List<CR_AccessRightsByRole> AccessRightsByRoles, string RoleTypeProperty)
    {
      // Assume ViewBag contains the column name
      string roleTypeColumnName = RoleTypeProperty;
      if (string.IsNullOrEmpty(roleTypeColumnName))
      {
        TempData["ErrorMessage"] = "Invalid RoleTypeProperty column name.";
        return BadRequest("RoleTypeProperty is missing.");
      }

      int numericRoleType = ExtractNumericValue(roleTypeColumnName);

      foreach (var setup in AccessRightsByRoles)
      {
        _logger.LogInformation("Received Setup: RoleTypeColumn={RoleTypeColumn}, ActionSOR={ActionSOR}", numericRoleType, setup.ActionSOR);
      }

      if (AccessRightsByRoles == null || AccessRightsByRoles.Count == 0)
      {
        TempData["ErrorMessage"] = "No data received.";
        _logger.LogWarning("No data received for edit.");
        return Json(new { success = false, message = "No data received." });
      }

      if (ModelState.IsValid)
      {
        try
        {
          foreach (var setup in AccessRightsByRoles)
          {
            if (setup.ActionSOR != 0)
            {

              var propertyInfo = setup.GetType().GetProperty(roleTypeColumnName);
              if (propertyInfo != null && propertyInfo.CanWrite)
              {
                propertyInfo.SetValue(setup, 1);  // Set the value dynamically
              }
              _appDBContext.CR_AccessRightsByRoles.Update(setup);
            }
          }

          await _appDBContext.SaveChangesAsync();
          TempData["SuccessMessage"] = "Access Rights By Role Setup updated successfully.";
          return Json(new { success = true });
        }
        catch (Exception ex)
        {
          TempData["ErrorMessage"] = "An error occurred while updating the data.";
          _logger.LogError(ex, "Error updating Access Rights By Role Setups");
          return Json(new { success = false, message = "An error occurred while updating the data." });
        }
      }

      var errors = ModelState.Values.SelectMany(v => v.Errors);

      return PartialView("~/Views/Setup/AccessRightsByRole/EditAccessRightsByRole.cshtml", AccessRightsByRoles);
    }

    // Helper function to extract numeric value
    private int ExtractNumericValue(string input)
    {
      if (string.IsNullOrEmpty(input))
        return 0;

      // Extract numeric part using regex
      var match = System.Text.RegularExpressions.Regex.Match(input, @"\d+");
      return match.Success ? int.Parse(match.Value) : 0;
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

using Exampler_ERP.Models.Temp;
using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;


namespace Exampler_ERP.Controllers.Setup
{
  public class AccessRightsByRoleController : PositionController
  {
    private readonly AppDBContext _appDBContext;
    private readonly IStringLocalizer<AccessRightsByRoleController> _localizer;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AccessRightsByRoleController> _logger;
    private readonly Utils _utils;
    private readonly IHubContext<NotificationHub> _hubContext;

    public AccessRightsByRoleController(AppDBContext appDBContext, IConfiguration configuration, ILogger<AccessRightsByRoleController> logger, Utils utils, IHubContext<NotificationHub> hubContext, IStringLocalizer<AccessRightsByRoleController> localizer) 
    : base(appDBContext)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _logger = logger;
      _utils = utils;
      _hubContext = hubContext;
      _localizer = localizer;

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
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "No Role found with the name '" + searchRoleName + "'. Please check the name and try again.");
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

      var moduleNames = new Dictionary<int, string>
{
    { 0, "Module" },
    { 1, "HR" },
    { 2, "Finance" },
    { 3, "Store" },
    { 4, "Purchase" },
    { 5, "Setup" },
    { 6, "Setting" }
};
      var menuNames = new Dictionary<int, string>
{
    { 0, "Main Menu" },
    { 1, "Master Information" },
    { 2, "Employeement" },
    { 3, "HR" },
    { 4, "Finance" },
    { 5, "Report" }
};
      var actiontypeNames = new Dictionary<int, string>
{
    { 0, "Menu" },
    { 1, "Page" },
    { 2, "Button" }
};

      ViewBag.AccessRoleList = await _appDBContext.CR_AccessRightsByRoles
         .Select(r => new
         {
           r.ActionSOR,
           r.ActionName,
           r.ModuleID,
           ModuleName = moduleNames.ContainsKey(r.ModuleID) ? moduleNames[r.ModuleID] : "Unknown",
           r.MenuID,
           MenuName = menuNames.ContainsKey(r.MenuID) ? menuNames[r.MenuID] : "Unknown",
           r.ActionType,
           ActionTypeName = actiontypeNames.ContainsKey(r.ActionType) ? actiontypeNames[r.ActionType] : "Unknown"


         })
         .ToListAsync();


      return PartialView("~/Views/Setup/AccessRightsByRole/EditAccessRightsByRole.cshtml", accessRightsByRoles);
    }
    public async Task<IActionResult> GetRoleRow(string actionSOR)
    {
      var data = await _appDBContext.CR_AccessRightsByRoles
           .Where(u => u.ActionSOR.ToString() == actionSOR)
           .FirstOrDefaultAsync();

      if (data == null)
      {
        return Json(new { success = false, message = "No data found" });
      }

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
    public async Task<IActionResult> Edit(List<CR_AccessRightsByRole> accessRightsByRoles, string RoleTypeProperty)
    {
      string roleTypeColumnName = RoleTypeProperty;
      if (string.IsNullOrEmpty(roleTypeColumnName))
      {
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "Invalid RoleTypeProperty column name.");
        return BadRequest("RoleTypeProperty is missing.");
      }

      int numericRoleType = ExtractNumericValue(roleTypeColumnName);

      foreach (var setup in accessRightsByRoles)
      {
        _logger.LogInformation("Received Setup: RoleTypeColumn={RoleTypeColumn}, ActionSOR={ActionSOR}", numericRoleType, setup.ActionSOR);
      }

      if (accessRightsByRoles == null || accessRightsByRoles.Count == 0)
      {
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "No data received.");
        _logger.LogWarning("No data received for edit.");
        return Json(new { success = false, message = "No data received." });
      }

      if (ModelState.IsValid)
      {
        try
        {
          foreach (var setup in accessRightsByRoles)
          {
            if (setup.ActionSOR != 0)
            {
              var propertyInfo = setup.GetType().GetProperty(roleTypeColumnName);
              if (propertyInfo != null && propertyInfo.CanWrite)
              {
                if (propertyInfo.PropertyType == typeof(int?))
                {
                  propertyInfo.SetValue(setup, 1);
                }

                _appDBContext.Entry(setup).Property(roleTypeColumnName).IsModified = true;
              }
            }
          }

          await _appDBContext.SaveChangesAsync();
          await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Access Rights By Role Setup updated successfully.");
          return Json(new { success = true });
        }
        catch (Exception ex)
        {
          await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "An error occurred while updating the data.");
          _logger.LogError(ex, "Error updating Access Rights By Role Setups");
          return Json(new { success = false, message = "An error occurred while updating the data." });
        }
      }

      var errors = ModelState.Values.SelectMany(v => v.Errors);

      return PartialView("~/Views/Setup/AccessRightsByRole/EditAccessRightsByRole.cshtml", accessRightsByRoles);
    }

    private int ExtractNumericValue(string input)
    {
      if (string.IsNullOrEmpty(input))
        return 0;

      var match = System.Text.RegularExpressions.Regex.Match(input, @"\d+");
      return match.Success ? int.Parse(match.Value) : 0;
    }
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
      var roleTypeProperty = $"_{id}";

      var accessRightsByRoles = await _appDBContext.CR_AccessRightsByRoles
          .Where(u => EF.Property<int?>(u, roleTypeProperty) == 1)
          .ToListAsync();

      if (accessRightsByRoles.Any())
      {
        foreach (var setup in accessRightsByRoles)
        {
          var propertyInfo = setup.GetType().GetProperty(roleTypeProperty);
          if (propertyInfo != null && propertyInfo.CanWrite)
          {
            propertyInfo.SetValue(setup, 0);
          }

          _appDBContext.Entry(setup).Property(roleTypeProperty).IsModified = true;
        }

        await _appDBContext.SaveChangesAsync();

        await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Access Rights By Role Setups deleted successfully.");
        return Json(new { success = true });
      }

      await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "No matching records found for deletion.");
      return Json(new { success = false, message = "No matching records found." });
    }



    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var ProcessTypeApprovalSetups = await _appDBContext.CR_ProcessTypeApprovalSetups
      .Distinct()
      .Include(d => d.ProcessType)
      .ToListAsync();


      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add("ProcessTypeApprovalSetups");
        worksheet.Cells["A1"].Value = "ProcessType SetupID";
        worksheet.Cells["B1"].Value = _localizer["lbl_ProcessTypeName"];
        worksheet.Cells["C1"].Value = _localizer["lbl_Rank"];
        worksheet.Cells["D1"].Value = _localizer["lbl_RoleType"];


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
      .Include(d => d.ProcessType)
      .ToListAsync();
      return View("~/Views/HR/MasterInfo/ProcessTypeApprovalSetup/PrintProcessTypeApprovalSetups.cshtml", ProcessTypeApprovalSetups);
    }

  }
}

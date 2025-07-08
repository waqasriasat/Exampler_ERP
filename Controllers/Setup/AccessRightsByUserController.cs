using Exampler_ERP.Models.Temp;
using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Exampler_ERP.Controllers.Setup
{
  public class AccessRightsByUserController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AccessRightsByUserController> _logger;
    private readonly Utils _utils;
private readonly IHubContext<NotificationHub> _hubContext;

    public AccessRightsByUserController(AppDBContext appDBContext, IConfiguration configuration, ILogger<AccessRightsByUserController> logger, Utils utils, IHubContext<NotificationHub> hubContext)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _logger = logger;
      _utils = utils;
_hubContext = hubContext;
 
    }
    public async Task<IActionResult> Index(string searchUserName)
    {
      var usersQuery = _appDBContext.CR_Users.AsQueryable();

      if (!string.IsNullOrEmpty(searchUserName))
      {
        usersQuery = usersQuery
            .Where(pt => pt.UserName.Contains(CR_CipherKey.Encrypt(searchUserName)));


      }

      var users = await usersQuery.ToListAsync();
      var AccessRightsByUser = await _appDBContext.CR_AccessRightsByUsers.ToListAsync();

      var viewModel = new AccessRightsByUserListViewModel
      {
        AccessRightsWithUserCount = users.Select(pt => new AccessRightsWithUserCountViewModel
        {
          UserID = pt.UserID,
          UserName = CR_CipherKey.Decrypt(pt.UserName),
          TotalAccessCount = AccessRightsByUser.Count(ptf => ptf.UserID == pt.UserID)
        }).ToList()
      };

      if (!string.IsNullOrEmpty(searchUserName) && viewModel.AccessRightsWithUserCount.Count == 0)
      {
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "No User Name found with the name '" + searchUserName + "'. Please check the name and try again.");
      }

      return View("~/Views/Setup/AccessRightsByUser/AccessRightsByUser.cshtml", viewModel);
    }


    public async Task<IActionResult> Edit(int id)
    {
      var accessRightsByUsers = await _appDBContext.CR_AccessRightsByUsers
          .Where(p => p.UserID == id)
          .Include(pt => pt.user)
          .ToListAsync();

      var user = await _appDBContext.CR_Users
    .FirstOrDefaultAsync(u => u.UserID == id);

      if (user != null && !string.IsNullOrEmpty(user.UserName))
      {
        // Decrypt the UserName
        ViewBag.UserName = CR_CipherKey.Decrypt(user.UserName);
      }
      else
      {
        ViewBag.UserName = "User Not Found"; // Fallback message if user doesn't exist
      }

      if (accessRightsByUsers == null || !accessRightsByUsers.Any())
      {
        var accessRightByRole = await _appDBContext.CR_AccessRightsByRoles.FirstOrDefaultAsync();

        if (accessRightByRole != null)
        {
          accessRightsByUsers = new List<CR_AccessRightsByUser>
            {
                new CR_AccessRightsByUser
                {
                    ActionSOR = accessRightByRole.ActionSOR,
                    ActionName = accessRightByRole.ActionName,
                    ActionType = accessRightByRole.ActionType,
                    AccessRightID = accessRightByRole.AccessRightID,
                    MenuID = accessRightByRole.MenuID,
                    ModuleID = accessRightByRole.ModuleID,
                    UserID = id // Associate with the current user ID
                }
            };
        }
        else
        {
          return NotFound("Access rights by role not found.");
        }
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

      return PartialView("~/Views/Setup/AccessRightsByUser/EditAccessRightsByUser.cshtml", accessRightsByUsers);
    }




    [HttpPost]
    public async Task<IActionResult> Edit(List<CR_AccessRightsByUser> accessRightsByUsers)
    {
      foreach (var setup in accessRightsByUsers)
      {
        _logger.LogInformation("Received Setup: UserID={UserID}, ActionSOR={ActionSOR}", setup.UserID, setup.ActionSOR);
      }

      if (accessRightsByUsers == null || accessRightsByUsers.Count == 0)
      {
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "No data received.");
        _logger.LogWarning("No data received for edit.");
        return Json(new { success = false, message = "No data received." });
      }

      if (ModelState.IsValid)
      {
        try
        {
          var accessRightsByUser = accessRightsByUsers.FirstOrDefault()?.UserID;
          if (accessRightsByUser == null)
          {
            await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "Invalid accessRightsByUser.");
            return BadRequest("Invalid accessRightsByUser.");
          }

          var existingRecords = await _appDBContext.CR_AccessRightsByUsers
                                        .Where(p => p.UserID == accessRightsByUser)
                                        .ToListAsync();

          _appDBContext.CR_AccessRightsByUsers.RemoveRange(existingRecords);

          foreach (var setup in accessRightsByUsers)
          {
            if (setup.ActionSOR != null)
            {
              _appDBContext.CR_AccessRightsByUsers.Add(setup);
            }
          }

          await _appDBContext.SaveChangesAsync();
          await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Access Rights By User Setup Update successfully.");
          return Json(new { success = true });
        }
        catch (Exception ex)
        {
          await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "An error occurred while updating the data.");
          _logger.LogError(ex, "Error updating Access Rights By User");
          return Json(new { success = false, message = "An error occurred while updating the data." });
        }
      }

      var errors = ModelState.Values.SelectMany(v => v.Errors);
      return PartialView("~/Views/Setup/AccessRightsByUser/EditAccessRightsByUser.cshtml", accessRightsByUsers);
    }



    [HttpPost]
    public IActionResult Delete(int id)
    {
      var setups = _appDBContext.CR_AccessRightsByUsers
        .Where(x => x.UserID == id)
        .ToList();
      if (setups == null)
      {
        return Json(new { success = false, message = "Setup not found" });
      }

      _appDBContext.CR_AccessRightsByUsers.RemoveRange(setups);
      _appDBContext.SaveChanges();
      _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Access Rights By Users Setups deleted successfully.");
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

using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace Exampler_ERP.Controllers.HR.MasterInfo
{
  public class UserController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;

    public UserController(AppDBContext appDBContext, IConfiguration configuration, Utils utils)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
    }
    public async Task<IActionResult> Index(string searchUserName)
    {
      var usersQuery = _appDBContext.CR_Users
          .Where(b => b.DeleteYNID != 1)
          .Include(d => d.RoleType);

      var users = await usersQuery.ToListAsync();

      var decryptedUsers = users.Select(user => new CR_User
      {
        UserID = user.UserID,
        UserName = CR_CipherKey.Decrypt(user.UserName), // Decrypt the UserName
        ActiveYNID = user.ActiveYNID,
        RoleTypeID = user.RoleTypeID,
        RoleType = user.RoleType // Preserve RoleType information
      }).ToList();

      if (!string.IsNullOrEmpty(searchUserName))
      {
        decryptedUsers = decryptedUsers
            .Where(user => user.UserName.Contains(searchUserName))
            .ToList();
      }

      return View("~/Views/HR/MasterInfo/User/User.cshtml", decryptedUsers);
    }



    public async Task<IActionResult> Edit(int id)
    {
      ViewBag.RoleList = await _utils.GetRoles();
      ViewBag.EmployeeList = await _utils.GetEmployee();
      var user = await _appDBContext.CR_Users.FindAsync(id);
      if (user == null)
      {
        return NotFound();
      }

      // Decrypt UserName and Password
      user.UserName = CR_CipherKey.Decrypt(user.UserName);
      user.Password = CR_CipherKey.Decrypt(user.Password); // Assuming you have a Password field

      return PartialView("~/Views/HR/MasterInfo/User/EditUser.cshtml", user);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(CR_User user)
    {
      if (ModelState.IsValid)
      {
        if (string.IsNullOrEmpty(user.UserName) && string.IsNullOrEmpty(user.Password) && user.RoleTypeID.ToString().Length>0)
        {
          return Json(new { success = false, message = "user Name, Password and Role Name field is required. Please enter a valid text value." });
        }
        var userInDb = await _appDBContext.CR_Users.FindAsync(user.UserID);
        if (userInDb == null)
        {
          return NotFound();
        }

        // Encrypt UserName and Password before saving
        userInDb.UserName = CR_CipherKey.Encrypt(user.UserName);
        userInDb.Password = CR_CipherKey.Encrypt(user.Password);
        userInDb.RoleTypeID = user.RoleTypeID;
        userInDb.ActiveYNID = user.ActiveYNID;

        await _appDBContext.SaveChangesAsync();
        TempData["SuccessMessage"] = "User Updated successfully.";
        return Json(new { success = true });
      }
     
      ViewBag.RoleList = await _utils.GetRoles();
      ViewBag.EmployeeList = await _utils.GetEmployee();
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      return Json(new { success = false, message = "Error creating user. Please check the inputs." });
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.RoleList = await _utils.GetRoles();
      ViewBag.EmployeeList = await _utils.GetEmployee();
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      return PartialView("~/Views/HR/MasterInfo/User/AddUser.cshtml", new CR_User());
    }

    [HttpPost]
    public async Task<IActionResult> Create(CR_User User)
    {
      if (ModelState.IsValid)
      {
        if (string.IsNullOrEmpty(User.UserName) && string.IsNullOrEmpty(User.Password) && User.RoleTypeID.ToString().Length > 0)
        {
          return Json(new { success = false, message = "user Name, Password and Role Name field is required. Please enter a valid text value." });
        }
        User.UserName = CR_CipherKey.Encrypt(User.UserName);
        User.Password = CR_CipherKey.Encrypt(User.Password);

        User.ActiveYNID = 2;
        User.DeleteYNID = 0;

        _appDBContext.CR_Users.Add(User);
        await _appDBContext.SaveChangesAsync();

        var userId = User.UserID;
        if (userId > 0)
        {
          var processCount = await _appDBContext.CR_ProcessTypeApprovalSetups
                              .Where(pta => pta.ProcessTypeID > 0 && pta.ProcessTypeID == 1)
                              .CountAsync();

          if (processCount > 0)
          {
            var newProcessTypeApproval = new CR_ProcessTypeApproval
            {
              ProcessTypeID = 1,
              Notes = "Create New User",
              Date = DateTime.Now,
              EmployeeID = User.EmployeeID ?? 0,
              UserID = HttpContext.Session.GetInt32("UserID") ?? default(int),
              TransactionID = User.UserID
            };

            _appDBContext.CR_ProcessTypeApprovals.Add(newProcessTypeApproval);
            await _appDBContext.SaveChangesAsync();

            var nextApprovalSetup = await _appDBContext.CR_ProcessTypeApprovalSetups
                                        .Where(pta => pta.ProcessTypeID == 1 && pta.Rank == 1)
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
            }
            else
            {
              return Json(new { success = false, message = "Next approval setup not found." });
            }
          }
          else
          {
            User.ActiveYNID = 1;
            _appDBContext.CR_Users.Update(User);
            await _appDBContext.SaveChangesAsync();
            TempData["SuccessMessage"] = "User Created successfully. No process setup found, User activated.";
            return Json(new { success = true});
          }
        }
        TempData["SuccessMessage"] = "User Created successfully. Continue to the Approval Process Setup for User Activation.";
        return Json(new { success = true });
      }
      return Json(new { success = false, message = "Error creating user. Please check the inputs." });
    }

    public async Task<IActionResult> Delete(int id)
    {
      var User = await _appDBContext.CR_Users.FindAsync(id);
      if (User == null)
      {
        return NotFound();
      }

      User.ActiveYNID = 2;
      User.DeleteYNID = 1;

      _appDBContext.CR_Users.Update(User);
      await _appDBContext.SaveChangesAsync();
      TempData["SuccessMessage"] = "User Deleted successfully.";
      return Json(new { success = true });
    }
    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var Users = await _appDBContext.CR_Users
        .Where(b => b.DeleteYNID != 1)
        .Include(d => d.RoleType)
        .ToListAsync();

      var decryptedUsers = Users.Select(user => new CR_User
      {
        UserID = user.UserID,
        UserName = CR_CipherKey.Decrypt(user.UserName),
        ActiveYNID = user.ActiveYNID,
        RoleTypeID = user.RoleTypeID,
        RoleType = user.RoleType // Include the role information
      }).ToList();

      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add("decryptedUsers");
        worksheet.Cells["A1"].Value = "User ID";
        worksheet.Cells["B1"].Value = "User Name";
        worksheet.Cells["C1"].Value = "Role Name";
        worksheet.Cells["D1"].Value = "Active";


        for (int i = 0; i < decryptedUsers.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = decryptedUsers[i].UserID;
          worksheet.Cells[i + 2, 2].Value = decryptedUsers[i].UserName;
          worksheet.Cells[i + 2, 3].Value = decryptedUsers[i].RoleType?.RoleTypeName;
          worksheet.Cells[i + 2, 4].Value = decryptedUsers[i].ActiveYNID == 1 ? "Yes" : "No";
        }

        worksheet.Cells["A1:l1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = $"Users-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
    public async Task<IActionResult> Print()
    {
      var Users = await _appDBContext.CR_Users
        .Where(b => b.DeleteYNID != 1)
        .Include(d => d.RoleType) // Eagerly load the related Role data
        .ToListAsync();

      var decryptedUsers = Users.Select(user => new CR_User
      {
        UserID = user.UserID,
        UserName = CR_CipherKey.Decrypt(user.UserName),
        ActiveYNID = user.ActiveYNID,
        RoleTypeID = user.RoleTypeID,
        RoleType = user.RoleType // Include the role information
      }).ToList();
      return View("~/Views/HR/MasterInfo/User/PrintUsers.cshtml", decryptedUsers);
    }

  }
}

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
    public async Task<IActionResult> Index()
    {
      var users = await _appDBContext.CR_Users
          .Where(b => b.DeleteYNID != 1)
          .Include(d => d.Role)
          .ToListAsync();

      var decryptedUsers = users.Select(user => new CR_User
      {
        UserID = user.UserID,
        UserName = CR_CipherKey.Decrypt(user.UserName),
        ActiveYNID = user.ActiveYNID,
        RoleID = user.RoleID,
        Role = user.Role // Include the role information
      }).ToList();

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
        var userInDb = await _appDBContext.CR_Users.FindAsync(user.UserID);
        if (userInDb == null)
        {
          return NotFound();
        }

        // Encrypt UserName and Password before saving
        userInDb.UserName = CR_CipherKey.Encrypt(user.UserName);
        userInDb.Password = CR_CipherKey.Encrypt(user.Password);
        userInDb.RoleID = user.RoleID;
        userInDb.ActiveYNID = user.ActiveYNID;

        await _appDBContext.SaveChangesAsync();
        return Json(new { success = true });
      }
      ViewBag.RoleList = await _utils.GetRoles();
      ViewBag.EmployeeList = await _utils.GetEmployee();
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      return PartialView("~/Views/HR/MasterInfo/User/EditUser.cshtml", user);
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
        User.UserName = CR_CipherKey.Encrypt(User.UserName);
        User.Password = CR_CipherKey.Encrypt(User.Password);

        User.ActiveYNID = 0;
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
            return Json(new { success = true, message = "No process setup found, User activated." });
          }
        }

        return Json(new { success = true });
      }

      return PartialView("~/Views/HR/MasterInfo/User/AddUser.cshtml", User);
    }

    public async Task<IActionResult> Delete(int id)
    {
      var User = await _appDBContext.CR_Users.FindAsync(id);
      if (User == null)
      {
        return NotFound();
      }

      User.ActiveYNID = 0;
      User.DeleteYNID = 1;

      _appDBContext.CR_Users.Update(User);
      await _appDBContext.SaveChangesAsync();

      return Json(new { success = true });
    }
    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var Users = await _appDBContext.CR_Users
        .Where(b => b.DeleteYNID != 1)
        .Include(d => d.Role)
        .ToListAsync();

      var decryptedUsers = Users.Select(user => new CR_User
      {
        UserID = user.UserID,
        UserName = CR_CipherKey.Decrypt(user.UserName),
        ActiveYNID = user.ActiveYNID,
        RoleID = user.RoleID,
        Role = user.Role // Include the role information
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
          worksheet.Cells[i + 2, 3].Value = decryptedUsers[i].Role?.RoleName;
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
        .Include(d => d.Role) // Eagerly load the related Role data
        .ToListAsync();

      var decryptedUsers = Users.Select(user => new CR_User
      {
        UserID = user.UserID,
        UserName = CR_CipherKey.Decrypt(user.UserName),
        ActiveYNID = user.ActiveYNID,
        RoleID = user.RoleID,
        Role = user.Role // Include the role information
      }).ToList();
      return View("~/Views/HR/MasterInfo/User/PrintUsers.cshtml", decryptedUsers);
    }

  }
}

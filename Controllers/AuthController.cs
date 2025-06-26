using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Models;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Exampler_ERP.Controllers
{
  public class AuthController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    public AuthController(AppDBContext appDBContext, IConfiguration configuration)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
    }
    [HttpGet]
    public IActionResult ForgotPassword() => View();
    public IActionResult Login() => View();
    public IActionResult Register() => View();
    public IActionResult EmployeeLogin() => View();
    public IActionResult SupplierLogin() => View();
    public IActionResult Logout()
    {
      if (HttpContext.Session.GetInt32("UserID") != null)
      {
        HttpContext.Session.Remove("UserID");
        return RedirectToAction("Login");
      }
      else
      {
        return RedirectToAction("Login");
      }
      return View();
    }
    [HttpPost]
    public async Task<IActionResult> Login(string Username, string Password)
    {
      if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
      {
        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return View();
      }

      // Hash the user-entered password
      string encryptedusername = CR_CipherKey.Encrypt(Username);
      string encryptedpassword = CR_CipherKey.Encrypt(Password);

      // Query the database for the user with the given username and hashed password
      var user = await _appDBContext.CR_Users
          .Where(u => u.UserName == encryptedusername && u.Password == encryptedpassword && u.ActiveYNID == 1 && u.DeleteYNID != 1)
          .FirstOrDefaultAsync();

      if (user == null)
      {
        TempData["ErrorMessage"] = "Wrong username or password";
        return View();
      }


      // Logic for setting up user session or authentication cookie goes here
      HttpContext.Session.SetInt32("UserID", user.UserID);
      HttpContext.Session.SetString("UserName", Username);
      HttpContext.Session.SetInt32("UserRoleID", user.RoleTypeID);
      HttpContext.Session.SetString("UserRoleName", Username);


      var accessRightbyUser = await _appDBContext.CR_AccessRightsByUsers
          .Where(u => u.UserID == user.UserID)
          .Select(u => u.ActionSOR)
          .ToListAsync();

      var usersJson = JsonSerializer.Serialize(accessRightbyUser);

      HttpContext.Session.SetString("AccessRightsByUserActionSORs", usersJson);


      var roleTypeProperty = $"_{user.RoleTypeID}";

      var accessRightbyRole = await _appDBContext.CR_AccessRightsByRoles
          .Where(u => EF.Property<int>(u, roleTypeProperty) == 1)
          .Select(u => u.ActionSOR)
          .ToListAsync();

      var accessRightsByRoleJson = JsonSerializer.Serialize(accessRightbyRole);

      HttpContext.Session.SetString("AccessRightsByRoleActionSORs", accessRightsByRoleJson);




      TempData["SuccessMessage"] = "Successfully Login. Wellcome to " + Username;
      return RedirectToAction("Index", "Dashboards");
    }

    [HttpPost]
    public async Task<IActionResult> EmployeeLogin(string Username, string Password)
    {
      if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
      {
        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return View();
      }

      string encryptedusername = CR_CipherKey.Encrypt(Username);
      string encryptedpassword = CR_CipherKey.Encrypt(Password);

      var employee = await _appDBContext.HR_Employees
    .Include(e => e.DepartmentType)
    .Include(e => e.DesignationType)
    .Where(e => e.UserName == encryptedusername
                && e.Password == encryptedpassword
                && e.ActiveYNID == 1
                && e.DeleteYNID != 1)
    .FirstOrDefaultAsync();

      if (employee == null)
      {
        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return View();
      }

      HttpContext.Session.SetInt32("EmployeeID", employee.EmployeeID);
      HttpContext.Session.SetString("EmployeeName", employee.FirstName + ' ' + employee.FatherName + ' ' + employee.FamilyName);
      if (employee.DepartmentType != null)
      {
        HttpContext.Session.SetString("EmployeeDepartmentName", employee.DepartmentType.DepartmentTypeName);
      }
      else
      {
        HttpContext.Session.SetString("EmployeeDepartmentName", "Unknown");
      }
      if (employee.DesignationType != null)
      {
        HttpContext.Session.SetString("EmployeeDesignationName", employee.DesignationType.DesignationTypeName);
      }
      else
      {
        HttpContext.Session.SetString("EmployeeDesignationName", "Unknown");
      }
    

      return RedirectToAction("Index", "EmployeeDashboards");
    }

    [HttpPost]
    public async Task<IActionResult> SupplierLogin(string Username, string Password)
    {
      if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
      {
        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return View();
      }

      // Hash the user-entered password
      string encryptedusername = CR_CipherKey.Encrypt(Username);
      string encryptedpassword = CR_CipherKey.Encrypt(Password);

      // Query the database for the user with the given username and hashed password
      var vendor = await _appDBContext.FI_Vendors
          .Include(u => u.HeadofAccount_Five)
          .Where(u => u.UserName == encryptedusername && u.Password == encryptedpassword && u.ActiveYNID == 1 && u.DeleteYNID != 1)
          .FirstOrDefaultAsync();

      if (vendor == null)
      {
        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return View();
      }


      // Logic for setting up user session or authentication cookie goes here
      HttpContext.Session.SetInt32("SupplierID", vendor.VendorID);
      if (vendor?.HeadofAccount_Five?.HeadofAccount_FiveName != null)
      {
        HttpContext.Session.SetString("SupplierName", vendor.HeadofAccount_Five.HeadofAccount_FiveName);
      }
  


      return RedirectToAction("Index", "SupplierDashboards");
    }
  }
}

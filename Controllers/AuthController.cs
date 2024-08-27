using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Models;
using System.Security.Cryptography;
using System.Text;

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
        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return View();
      }

      // Logic for setting up user session or authentication cookie goes here
      HttpContext.Session.SetInt32("UserID", user.UserID);
      HttpContext.Session.SetString("UserName", Username);
      HttpContext.Session.SetInt32("UserRoleID", user.RoleID);
      HttpContext.Session.SetString("UserRoleName", Username);



      return RedirectToAction("Index", "Dashboards");
    }
  }
}

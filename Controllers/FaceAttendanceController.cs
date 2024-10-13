using Exampler_ERP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Exampler_ERP.Controllers
{
  public class FaceAttendanceController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;

    public FaceAttendanceController(AppDBContext appDBContext, IConfiguration configuration)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
    }

    // Initial Load (without webcam)
    public IActionResult FaceAttendanceMarks()
    {
      return View();
    }

    // Fetch Employee Details by EmployeeID
    [HttpPost]
    public async Task<IActionResult> FetchEmployeeDetails([FromBody] EmployeeRequest request)
    {
      var employeeID = request.EmployeeID;

      var employee = await _appDBContext.HR_Employees
          .Where(emp => emp.EmployeeID == employeeID)
          .Select(emp => new
          {
            emp.EmployeeID,
            emp.UserName, // Assuming you need the username as well
            emp.FirstName,
            emp.FatherName,
            emp.FamilyName,
            Branch = emp.BranchType.BranchTypeName,
            Department = emp.DepartmentType.DepartmentTypeName,
            Designation = emp.DesignationType.DesignationTypeName
          })
          .FirstOrDefaultAsync();

      if (employee == null)
      {
        return Json(new { success = false, message = "Employee not found." });
      }

      return Json(new { success = true, employee });
    }

    // Verify Password and load Webcam
    [HttpPost]
    public async Task<IActionResult> VerifyPassword([FromBody] VerifyPasswordRequest request)
    {
      var employeeID = request.EmployeeID;
      var password = request.Password;

      var encryptedPassword = CR_CipherKey.Encrypt(password);
      var employee = await _appDBContext.HR_Employees
          .Where(emp => emp.EmployeeID == employeeID && emp.Password == encryptedPassword)
          .FirstOrDefaultAsync();

      if (employee == null)
      {
        return Json(new { success = false, message = "Invalid password." });
      }

      // Password is correct, allow webcam access
      return Json(new { success = true });
    }

    // Save attendance
    [HttpPost]
    public async Task<IActionResult> SubmitAttendance([FromBody] AttendanceRequest request)
    {
      var today = DateTime.Today;

      var existingAttendance = await _appDBContext.CR_FaceAttendances
          .Where(a => a.EmployeeID == request.EmployeeID && a.MarkDate == today && a.MarkSourceID==1)
          .FirstOrDefaultAsync();

      byte[] imageBytes = Convert.FromBase64String(request.ImageData.Substring(request.ImageData.IndexOf(",") + 1));

      if (existingAttendance != null)
      {
        // Update record for "out" if "in" was already marked
        if (existingAttendance.MarkSourceID == 1)
        {
          existingAttendance.MarkSourceID = 2;
          existingAttendance.OutTime = DateTime.Now;
          existingAttendance.OutPicture = imageBytes;
          await _appDBContext.SaveChangesAsync();
          return Json(new { success = true, message = "Attendance updated (out)." });
        }
      }

      // Create a new record for today's attendance with MarkSourceID = 1 ("in")
      var attendance = new CR_FaceAttendance
      {
        EmployeeID = request.EmployeeID,
        MarkDate = today,
        MarkSourceID = 1,
        InTime = DateTime.Now,
        InPicture = imageBytes
      };

      _appDBContext.CR_FaceAttendances.Add(attendance);
      await _appDBContext.SaveChangesAsync();

      return Json(new { success = true, message = "Attendance marked (in)." });
    }
  }

  // Request models
  public class EmployeeRequest
  {
    public int EmployeeID { get; set; }
  }

  public class VerifyPasswordRequest
  {
    public int EmployeeID { get; set; }
    public string Password { get; set; }
  }

  public class AttendanceRequest
  {
    public int EmployeeID { get; set; }
    public string ImageData { get; set; }
  }
}
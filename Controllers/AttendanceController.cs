using Exampler_ERP.Models;
using Microsoft.AspNetCore.Mvc;

namespace Exampler_ERP.Controllers
{
  public class AttendanceController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    public AttendanceController(AppDBContext appDBContext, IConfiguration configuration)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
    }
    public IActionResult AttendanceMarks()
    {
      return View();
    }
    [HttpPost]
    public async Task<IActionResult> AttendanceMarks(string Username, string Password)
    {
      return View();
    }
    [HttpPost]
    public IActionResult SubmitAttendance(string imageData)
    {
      if (!string.IsNullOrEmpty(imageData))
      {
        // Remove the "data:image/png;base64," part
        var base64Data = imageData.Substring(imageData.IndexOf(",") + 1);
        byte[] imageBytes = Convert.FromBase64String(base64Data);

        // Save the image to a database or perform any other processing
        // Example: Save to file (you can modify this part to save into the database)
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/attendance", $"{Guid.NewGuid()}.png");
        System.IO.File.WriteAllBytes(filePath, imageBytes);

        return Json(new { success = true });
      }

      return Json(new { success = false });
    }

  }
}

using Exampler_ERP.Models;
using Exampler_ERP.Models.Temp;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Localization;

namespace Exampler_ERP.Controllers.HR.Reports
{
  public class ExperienceCertificateController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IStringLocalizer<ExperienceCertificateController> _localizer;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    private readonly IHubContext<NotificationHub> _hubContext;

    public ExperienceCertificateController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext, IStringLocalizer<ExperienceCertificateController> localizer)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
      _hubContext = hubContext;
      _localizer = localizer;

    }
    public async Task<IActionResult> Index(int? id)
    {
      var employeesQuery = _appDBContext.HR_Employees
       .Where(c => c.DeleteYNID != 1 && c.FinalApprovalID == 1);

      if (id.HasValue)
      {
        employeesQuery = employeesQuery.Where(c => c.EmployeeID == id.Value);
      }

      var employees = await employeesQuery
          .Include(c => c.BranchType)
          .Include(c => c.DepartmentType)
          .ToListAsync();

      return View("~/Views/HR/Reports/ExperienceCertificate/ExperienceCertificate.cshtml", employees);
    }
    public async Task<IActionResult> Experience(int id)
    {
      var employeeExperiences = await _appDBContext.HR_EmployeeExperiences
          .Where(e => e.EmployeeID == id)
          .ToListAsync();

      return PartialView("~/Views/HR/Reports/ExperienceCertificate/ViewEmployeeExperience.cshtml", employeeExperiences);
    }
    [HttpGet]
    public async Task<IActionResult> ExperienceDocumentDownload(int id)
    {
      var document = await _appDBContext.HR_EmployeeExperiences
          .FirstOrDefaultAsync(d => d.ExperienceID == id);

      if (document == null || document.DocImage == null)
      {
        return NotFound();
      }

      return File(document.DocImage, "application/octet-stream", "(" + document.EmployeeID + ") " + document.CompanyName + document.DocExt);
    }
    [HttpGet]
    public async Task<IActionResult> ExperienceDocumentView(int id)
    {
      var document = await _appDBContext.HR_EmployeeExperiences
          .FirstOrDefaultAsync(d => d.ExperienceID == id);

      if (document == null || document.DocImage == null)
      {
        // If document is not found, return a 404 error.
        return NotFound();
      }

      // Determine MIME type based on file extension
      string mimeType = document.DocExt.ToLower() switch
      {
        ".pdf" => "application/pdf",   // Ensure PDF is returned correctly
        ".jpg" => "image/jpeg",
        ".jpeg" => "image/jpeg",
        ".png" => "image/png",
        _ => "application/octet-stream"
      };

      // Return the document for inline viewing
      return File(document.DocImage, mimeType, "(" + document.EmployeeID + ") " + document.CompanyName + document.DocExt);
    }
  }
}

using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Linq;
using System.Threading.Tasks;

namespace Exampler_ERP.Controllers.HR.Employeement
{
  public class ThumbEnrollmentController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly Utils _utils;
private readonly IHubContext<NotificationHub> _hubContext;


    public ThumbEnrollmentController(AppDBContext appDBContext, Utils utils, IHubContext<NotificationHub> hubContext)
    {
      _appDBContext = appDBContext;
      _utils = utils;
_hubContext = hubContext;
 
    }

    public async Task<IActionResult> Index(int? id) // EmployeeID
    {
      var ThumbEnrollmentQuery = _appDBContext.HR_ThumbEnrollments
         .Where(c => c.DeleteYNID != 1);

      if (id.HasValue)
      {
        ThumbEnrollmentQuery = ThumbEnrollmentQuery.Where(c => c.EmployeeID == id.Value);
      }

      var ThumbEnrollments = await ThumbEnrollmentQuery
          .Include(c => c.Employee).
          ToListAsync();

      return View("~/Views/HR/Employeement/ThumbEnrollment/ThumbEnrollment.cshtml", ThumbEnrollments);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.EmployeesList = await _utils.GetEmployee();
      return PartialView("~/Views/HR/Employeement/ThumbEnrollment/AddThumbEnrollment.cshtml", new HR_ThumbEnrollment());
    }

    [HttpPost]
    public async Task<IActionResult> Create(HR_ThumbEnrollment thumbEnrollment, IFormFile thumbImpression)
    {
      if (ModelState.IsValid)
      {
        _appDBContext.HR_ThumbEnrollments.Add(thumbEnrollment);
        await _appDBContext.SaveChangesAsync();
        return Json(new { success = true });
      }

      return Json(new { success = false });
    }
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
      var thumbEnrollment = await _appDBContext.HR_ThumbEnrollments
                                                 .FirstOrDefaultAsync(t => t.ThumbID == id);

      if (thumbEnrollment == null)
        return NotFound();

      ViewBag.Employees = new SelectList(_appDBContext.HR_Employees, "EmployeeID", "FirstName", thumbEnrollment.EmployeeID);
      return PartialView("_EditPartial", thumbEnrollment);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(HR_ThumbEnrollment thumbEnrollment)
    {
      if (ModelState.IsValid)
      {
        _appDBContext.HR_ThumbEnrollments.Update(thumbEnrollment);
        await _appDBContext.SaveChangesAsync();

        return Json(new { success = true });
      }

      return Json(new { success = false });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
      var thumbEnrollment = await _appDBContext.HR_ThumbEnrollments.FindAsync(id);

      if (thumbEnrollment == null)
        return Json(new { success = false });

      _appDBContext.HR_ThumbEnrollments.Remove(thumbEnrollment);
      await _appDBContext.SaveChangesAsync();

      return Json(new { success = true });
    }

    [HttpGet]
    public async Task<IActionResult> GetEmployeeSuggestions(string term)
    {
      var suggestions = await _appDBContext.HR_Employees
                                           .Where(e => e.FirstName.Contains(term) || e.FatherName.Contains(term))
                                           .Select(e => new { label = $"{e.FirstName} {e.FatherName}", id = e.EmployeeID })
                                           .ToListAsync();

      return Json(suggestions);
    }
  }
}

using Exampler_ERP.Models.Temp;
using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Exampler_ERP.Controllers.HR.HR
{
  public class AddionalAllowanceController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AddionalAllowanceController> _logger;
    private readonly Utils _utils;
    public AddionalAllowanceController(AppDBContext appDBContext, IConfiguration configuration, ILogger<AddionalAllowanceController> logger, Utils utils)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _logger = logger;
      _utils = utils;
    }
    public async Task<IActionResult> Index(int? MonthsTypeID, int? YearsTypeID, string? EmployeeName, int? EmployeeID)
    {
      var query = _appDBContext.HR_AddionalAllowances
        .Include(d => d.Employee)
        .Include(d => d.MonthType)
        .AsQueryable();

      if (MonthsTypeID.HasValue && MonthsTypeID != 0)
      {
        query = query.Where(d => d.MonthTypeID == MonthsTypeID.Value);
      }

      if (YearsTypeID.HasValue)
      {
        query = query.Where(d => d.Year == YearsTypeID.Value);
      }

      if (EmployeeID.HasValue)
      {
        query = query.Where(d => d.EmployeeID == EmployeeID.Value);
      }

      if (!string.IsNullOrEmpty(EmployeeName))
      {
        query = query.Where(d =>
            (d.Employee.FirstName + " " + d.Employee.FatherName + " " + d.Employee.FamilyName)
            .Contains(EmployeeName));
      }
      var addionalAllowances = await query.ToListAsync();

      ViewBag.MonthsTypeID = MonthsTypeID;
      ViewBag.YearsTypeID = YearsTypeID;
      ViewBag.EmployeeID = EmployeeID;
      ViewBag.EmployeeName = EmployeeName;

      ViewBag.MonthsTypeList = await _utils.GetMonthsTypes();

      return View("~/Views/HR/HR/AddionalAllowance/AddionalAllowance.cshtml", addionalAllowances);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
      // Fetch the allowance record with the specified id
      var allowance = await _appDBContext.HR_AddionalAllowances
          .Include(a => a.AddionalAllowanceDetails) // Include the details for editing
          .FirstOrDefaultAsync(a => a.AddionalAllowanceID == id);

      if (allowance == null)
      {
        return NotFound(); // Handle not found case
      }

      // Prepare necessary ViewBag data for the edit view
      ViewBag.AddionalAllowanceTypeList = await _appDBContext.Settings_AddionalAllowanceTypes
          .Select(r => new { Value = r.AddionalAllowanceTypeID, Text = r.AddionalAllowanceTypeName })
          .ToListAsync();

      ViewBag.EmployeesList = await _utils.GetEmployee();
      ViewBag.MonthsList = await _utils.GetMonthsTypes();

      return PartialView("~/Views/HR/HR/AddionalAllowance/EditAddionalAllowance.cshtml", allowance);
    }
    [HttpPost]
    public async Task<IActionResult> Edit(HR_AddionalAllowance model)
    {
      if (ModelState.IsValid)
      {
        // Get the existing allowance record including its details
        var existingAllowance = await _appDBContext.HR_AddionalAllowances
            .Include(a => a.AddionalAllowanceDetails)
            .FirstOrDefaultAsync(a => a.AddionalAllowanceID == model.AddionalAllowanceID);

        if (existingAllowance == null)
        {
          return NotFound(); // Handle the case where the allowance is not found
        }

        // Update the main allowance properties
        existingAllowance.EmployeeID = model.EmployeeID;
        existingAllowance.MonthTypeID = model.MonthTypeID;
        existingAllowance.Year = model.Year;
        // Add any other fields that need to be updated...

        // Compare and update detail entries
        foreach (var existingDetail in existingAllowance.AddionalAllowanceDetails.ToList())
        {
          // Check if the detail is in the incoming model
          var updatedDetail = model.AddionalAllowanceDetails
              .FirstOrDefault(d => d.AddionalAllowanceDetailID == existingDetail.AddionalAllowanceDetailID);

          if (updatedDetail == null)
          {
            // If the detail is not in the incoming model, remove it from the database
            _appDBContext.HR_AddionalAllowanceDetails.Remove(existingDetail);
          }
          else
          {
            // If it exists, update its properties
            existingDetail.AddionalAllowanceTypeID = updatedDetail.AddionalAllowanceTypeID;
            existingDetail.AddionalAllowanceAmount = updatedDetail.AddionalAllowanceAmount;
          }
        }

        // Add new details that are in the incoming model but not in the existing model
        foreach (var newDetail in model.AddionalAllowanceDetails)
        {
          if (newDetail.AddionalAllowanceDetailID == 0) // Assuming 0 means it's a new entry
          {
            existingAllowance.AddionalAllowanceDetails.Add(newDetail);
          }
        }

        // Save changes to the database
        await _appDBContext.SaveChangesAsync();

        return Json(new { success = true });
      }

      // If validation fails, return the view with errors
      ViewBag.AddionalAllowanceTypeList = await _appDBContext.Settings_AddionalAllowanceTypes
          .Select(r => new { Value = r.AddionalAllowanceTypeID, Text = r.AddionalAllowanceTypeName })
          .ToListAsync();

      ViewBag.EmployeesList = await _utils.GetEmployee();
      ViewBag.MonthsList = await _utils.GetMonthsTypes();

      return PartialView("~/Views/HR/HR/AddionalAllowance/EditAddionalAllowance.cshtml", model);
    }

 

    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.AddionalAllowanceTypeList = await _appDBContext.Settings_AddionalAllowanceTypes
          .Select(r => new { Value = r.AddionalAllowanceTypeID, Text = r.AddionalAllowanceTypeName })
          .ToListAsync();

      ViewBag.EmployeesList = await _utils.GetEmployee(); // Assuming this method fetches employee data
      ViewBag.MonthsList = await _utils.GetMonthsTypes(); // Assuming this method fetches month data

      return PartialView("~/Views/HR/HR/AddionalAllowance/AddAddionalAllowance.cshtml", new HR_AddionalAllowance());
    }

    [HttpPost]
    public async Task<IActionResult> Create(HR_AddionalAllowance model)
    {
     
        if (ModelState.IsValid)
        {
          _appDBContext.HR_AddionalAllowances.Add(model);
          await _appDBContext.SaveChangesAsync();

        int generatedAddionalAllowanceID = model.AddionalAllowanceID;
        if (generatedAddionalAllowanceID > 0)
        {
          var processCount = await _appDBContext.CR_ProcessTypeApprovalSetups
                              .Where(pta => pta.ProcessTypeID > 0 && pta.ProcessTypeID == 8)
                              .CountAsync();
          var getEmployeeID = await _appDBContext.HR_AddionalAllowances
                            .Where(pta => pta.AddionalAllowanceID == generatedAddionalAllowanceID)
                            .FirstOrDefaultAsync();
          if (processCount > 0)
          {
            var newProcessTypeApproval = new CR_ProcessTypeApproval
            {
              ProcessTypeID = 8,
              Notes = "Addional Allowance",
              Date = DateTime.Now,
              EmployeeID = getEmployeeID.EmployeeID,
              UserID = HttpContext.Session.GetInt32("UserID") ?? default(int),
              TransactionID = generatedAddionalAllowanceID
            };

            _appDBContext.CR_ProcessTypeApprovals.Add(newProcessTypeApproval);
            await _appDBContext.SaveChangesAsync();

            var nextApprovalSetup = await _appDBContext.CR_ProcessTypeApprovalSetups
                                        .Where(pta => pta.ProcessTypeID == 8 && pta.Rank == 1)
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
            model.FinalApprovalID = 1;
            _appDBContext.HR_AddionalAllowances.Update(model);
            await _appDBContext.SaveChangesAsync();
            return Json(new { success = true, message = "No process setup found, User activated." });
          }
        }

        return Json(new { success = true });
        }
     
      ViewBag.AddionalAllowanceTypeList = await _appDBContext.Settings_AddionalAllowanceTypes
          .Select(r => new { Value = r.AddionalAllowanceTypeID, Text = r.AddionalAllowanceTypeName })
          .ToListAsync();
      ViewBag.EmployeesList = await _utils.GetEmployee();
      ViewBag.MonthsList = await _utils.GetMonthsTypes();

      return PartialView("~/Views/HR/HR/AddionalAllowance/AddAddionalAllowance.cshtml", model);
    }


  }
}

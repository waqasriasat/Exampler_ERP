using Exampler_ERP.Models.Temp;
using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Exampler_ERP.Controllers.HR.MasterInfo
{
  public class EmployeeRequestTypeForwardController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmployeeRequestTypeForwardController> _logger;
    private readonly Utils _utils;
private readonly IHubContext<NotificationHub> _hubContext;

    public EmployeeRequestTypeForwardController(AppDBContext appDBContext, IConfiguration configuration, ILogger<EmployeeRequestTypeForwardController> logger, Utils utils, IHubContext<NotificationHub> hubContext)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _logger = logger;
      _utils = utils;
_hubContext = hubContext;
 
    }
    public async Task<IActionResult> Index(string searchEmployeeRequestTypeName)
    {
      // Query for EmployeeRequest Types with an optional search filter
      var EmployeeRequestTypesQuery = _appDBContext.Settings_EmployeeRequestTypes.AsQueryable();

      if (!string.IsNullOrEmpty(searchEmployeeRequestTypeName))
      {
        EmployeeRequestTypesQuery = EmployeeRequestTypesQuery
            .Where(pt => pt.EmployeeRequestTypeName.Contains(searchEmployeeRequestTypeName));
      }

      var EmployeeRequestTypes = await EmployeeRequestTypesQuery.ToListAsync();
      var EmployeeRequestTypeForwards = await _appDBContext.HR_EmployeeRequestTypeForwards.ToListAsync();

      // Create ViewModel based on query results
      var viewModel = new EmployeeRequestTypeForwardListViewModel
      {
        EmployeeRequestTypesWithRoleCount = EmployeeRequestTypes.Select(pt => new EmployeeRequestTypeWithRoleCountViewModel
        {
          EmployeeRequestTypeID = pt.EmployeeRequestTypeID,
          EmployeeRequestTypeName = pt.EmployeeRequestTypeName,
          RoleCount = EmployeeRequestTypeForwards.Count(ptf => ptf.EmployeeRequestTypeID == pt.EmployeeRequestTypeID)
        }).ToList()
      };

      if (!string.IsNullOrEmpty(searchEmployeeRequestTypeName) && viewModel.EmployeeRequestTypesWithRoleCount.Count == 0)
      {
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "No Employee Request Type found with the name '" + searchEmployeeRequestTypeName + "'. Please check the name and try again.");
      }

      return View("~/Views/HR/MasterInfo/EmployeeRequestTypeForward/EmployeeRequestTypeForward.cshtml", viewModel);
    }
    public async Task<IActionResult> Edit(int id)
    {
      var EmployeeRequestTypeForwards = await _appDBContext.HR_EmployeeRequestTypeForwards
       .Where(pt => pt.EmployeeRequestTypeID == id)
       .Include(pt => pt.EmployeeRequestType)
       .Include(pt => pt.RoleType)
       .ToListAsync();

      if (EmployeeRequestTypeForwards == null || !EmployeeRequestTypeForwards.Any())
      {
        var EmployeeRequestType = await _appDBContext.Settings_EmployeeRequestTypes
            .Where(p => p.EmployeeRequestTypeID == id)
            .FirstOrDefaultAsync();

        if (EmployeeRequestType != null)
        {
          EmployeeRequestTypeForwards = new List<HR_EmployeeRequestTypeForward>
            {
                new HR_EmployeeRequestTypeForward
                {
                    EmployeeRequestTypeID = EmployeeRequestType.EmployeeRequestTypeID,
                    EmployeeRequestType = EmployeeRequestType
                }
            };
        }
        else
        {
          return NotFound(); // Or handle accordingly if EmployeeRequestType is not found
        }
      }

      ViewBag.RoleList = await _appDBContext.Settings_RoleTypes
          .Select(r => new { Value = r.RoleTypeID, Text = r.RoleTypeName })
          .ToListAsync();

      ViewBag.EmployeeRequestTypes = await _appDBContext.Settings_EmployeeRequestTypes.ToListAsync();

      return PartialView("~/Views/HR/MasterInfo/EmployeeRequestTypeForward/EditEmployeeRequestTypeForward.cshtml", EmployeeRequestTypeForwards);
    }



    [HttpPost]
    public async Task<IActionResult> Edit(List<HR_EmployeeRequestTypeForward> EmployeeRequestTypeForwards)
    {
      foreach (var setup in EmployeeRequestTypeForwards)
      {
        _logger.LogInformation("Received Setup: ID={ID}, RoleID={RoleID}", setup.EmployeeRequestTypeForwardID, setup.RoleTypeID);
      }

      if (EmployeeRequestTypeForwards == null || EmployeeRequestTypeForwards.Count == 0)
      {
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "No data received.");
        _logger.LogWarning("No data received for edit.");
        return Json(new { success = false, message = "No data received." });
      }

      if (ModelState.IsValid)
      {
        try
        {
          var EmployeeRequestTypeId = EmployeeRequestTypeForwards.FirstOrDefault()?.EmployeeRequestTypeID;
          if (EmployeeRequestTypeId == null)
          {
            await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "Invalid EmployeeRequestTypeID.");
            return BadRequest("Invalid EmployeeRequestTypeID.");
          }

          var existingRecords = await _appDBContext.HR_EmployeeRequestTypeForwards
                                        .Where(p => p.EmployeeRequestTypeID == EmployeeRequestTypeId)
                                        .ToListAsync();

          _appDBContext.HR_EmployeeRequestTypeForwards.RemoveRange(existingRecords);

          foreach (var setup in EmployeeRequestTypeForwards)
          {
            if (setup.RoleTypeID != 0)
            {
              _appDBContext.HR_EmployeeRequestTypeForwards.Add(setup);
            }
          }

          await _appDBContext.SaveChangesAsync();
          await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Employee Request Type Forward Update successfully.");
          return Json(new { success = true });
        }
        catch (Exception ex)
        {
          await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "An error occurred while updating the data.");
          _logger.LogError(ex, "Error updating EmployeeRequestTypeForwards");
          return Json(new { success = false, message = "An error occurred while updating the data." });
        }
      }

      var errors = ModelState.Values.SelectMany(v => v.Errors);
      return PartialView("~/Views/HR/MasterInfo/EmployeeRequestTypeForward/EditEmployeeRequestTypeForward.cshtml", EmployeeRequestTypeForwards);
    }



    [HttpPost]
    public IActionResult Delete(int id)
    {
      var setups = _appDBContext.HR_EmployeeRequestTypeForwards
        .Where(x => x.EmployeeRequestTypeID == id)
        .ToList(); 
      if (setups == null)
      {
        return Json(new { success = false, message = "Setup not found" });
      }

      _appDBContext.HR_EmployeeRequestTypeForwards.RemoveRange(setups);
      _appDBContext.SaveChanges();
       _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Employee Request Type Forward deleted successfully.");
      return Json(new { success = true });
    }


    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var EmployeeRequestTypeForwards = await _appDBContext.HR_EmployeeRequestTypeForwards
      .Distinct()
      .Include(d => d.EmployeeRequestType) // Eagerly load the related Branch data
      .ToListAsync();


      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add("EmployeeRequestTypeForwards");
        worksheet.Cells["A1"].Value = "EmployeeRequestType ApprovalID";
        worksheet.Cells["B1"].Value = "EmployeeRequestType Name";
        worksheet.Cells["C1"].Value = "Role";


        for (int i = 0; i < EmployeeRequestTypeForwards.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = EmployeeRequestTypeForwards[i].EmployeeRequestTypeForwardID;
          worksheet.Cells[i + 2, 2].Value = EmployeeRequestTypeForwards[i].EmployeeRequestType?.EmployeeRequestTypeName;
          worksheet.Cells[i + 2, 3].Value = EmployeeRequestTypeForwards[i].RoleTypeID;
        }

        worksheet.Cells["A1:l1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = $"EmployeeRequestTypeForwards-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
    public async Task<IActionResult> Print()
    {
      var EmployeeRequestTypeForwards = await _appDBContext.HR_EmployeeRequestTypeForwards
      .Distinct()
      .Include(d => d.EmployeeRequestType) // Eagerly load the related Branch data
      .ToListAsync();
      return View("~/Views/HR/MasterInfo/EmployeeRequestTypeForward/PrintEmployeeRequestTypeForwards.cshtml", EmployeeRequestTypeForwards);
    }

    //public async Task<IActionResult> Create()
    //{
    //  ViewBag.EmployeeRequestTypes = await _appDBContext.Settings_EmployeeRequestTypes.ToListAsync();
    //  ViewBag.Roles = await _appDBContext.Settings_RoleTypes.ToListAsync();
    //  return PartialView("~/Views/HR/MasterInfo/EmployeeRequestTypeForward/AddEmployeeRequestTypeForward.cshtml");
    //}

    //[HttpPost]
    //public async Task<IActionResult> Create(EmployeeRequestTypeForwardViewModel model)
    //{
    //  if (ModelState.IsValid)
    //  {
    //    foreach (var roleId in model.SelectedRoleIds)
    //    {
    //      var EmployeeRequestTypeForward = new HR_EmployeeRequestTypeForward
    //      {
    //        EmployeeRequestTypeID = model.EmployeeRequestTypeID,
    //        RoleTypeID = roleId
    //      };
    //      _appDBContext.HR_EmployeeRequestTypeForwards.Add(EmployeeRequestTypeForward);
    //    }
    //    await _appDBContext.SaveChangesAsync();
    //    return RedirectToAction(nameof(Index));
    //  }

    //  // Log the validation errors
    //  foreach (var key in ModelState.Keys)
    //  {
    //    var state = ModelState[key];
    //    foreach (var error in state.Errors)
    //    {
    //      // Use your logging mechanism here
    //      Console.WriteLine($"Validation error in {key}: {error.ErrorMessage}");
    //    }
    //  }

    //  ViewBag.EmployeeRequestTypes = await _appDBContext.Settings_EmployeeRequestTypes.ToListAsync();
    //  ViewBag.Roles = await _appDBContext.Settings_RoleTypes.ToListAsync();
    //  return PartialView("~/Views/HR/MasterInfo/EmployeeRequestTypeForward/AddEmployeeRequestTypeForward.cshtml", model);
    //}

    //public async Task<IActionResult> Edit(int id)
    //{
    //  var EmployeeRequestType = await _appDBContext.Settings_EmployeeRequestTypes.FindAsync(id);
    //  if (EmployeeRequestType == null)
    //  {
    //    return NotFound();
    //  }

    //  var selectedRoleIds = await _appDBContext.HR_EmployeeRequestTypeForwards
    //      .Where(ptf => ptf.EmployeeRequestTypeID == id)
    //      .Select(ptf => ptf.RoleTypeID)
    //      .ToListAsync();

    //  var viewModel = new EmployeeRequestTypeForwardViewModel
    //  {
    //    EmployeeRequestTypeID = id,
    //    SelectedRoleIds = selectedRoleIds
    //  };

    //  ViewBag.EmployeeRequestTypes = await _appDBContext.Settings_EmployeeRequestTypes.ToListAsync();
    //  ViewBag.Roles = await _appDBContext.Settings_RoleTypes.ToListAsync();
    //  return PartialView("~/Views/HR/MasterInfo/EmployeeRequestTypeForward/EditEmployeeRequestTypeForward.cshtml", viewModel);
    //}


    //[HttpPost]
    //public async Task<IActionResult> Edit(EmployeeRequestTypeForwardViewModel model)
    //{
    //  if (ModelState.IsValid)
    //  {
    //    // Remove existing entries
    //    var existingEntries = _appDBContext.HR_EmployeeRequestTypeForwards.Where(ptf => ptf.EmployeeRequestTypeID == model.EmployeeRequestTypeID);
    //    _appDBContext.HR_EmployeeRequestTypeForwards.RemoveRange(existingEntries);

    //    // Add new entries
    //    foreach (var roleId in model.SelectedRoleIds)
    //    {
    //      var EmployeeRequestTypeForward = new HR_EmployeeRequestTypeForward
    //      {
    //        EmployeeRequestTypeID = model.EmployeeRequestTypeID,
    //        RoleTypeID = roleId
    //      };
    //      _appDBContext.HR_EmployeeRequestTypeForwards.Add(EmployeeRequestTypeForward);
    //    }

    //    await _appDBContext.SaveChangesAsync();
    //    return RedirectToAction(nameof(Index));
    //  }

    //  // Log the validation errors
    //  foreach (var key in ModelState.Keys)
    //  {
    //    var state = ModelState[key];
    //    foreach (var error in state.Errors)
    //    {
    //      _logger.LogError($"Validation error in {key}: {error.ErrorMessage}");
    //    }
    //  }

    //  ViewBag.EmployeeRequestTypes = await _appDBContext.Settings_EmployeeRequestTypes.ToListAsync();
    //  ViewBag.Roles = await _appDBContext.Settings_RoleTypes.ToListAsync();
    //  return PartialView("~/Views/HR/MasterInfo/EmployeeRequestTypeForward/EditEmployeeRequestTypeForward.cshtml", model);
    //}
  }
}

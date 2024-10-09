using Exampler_ERP.Models.Temp;
using Exampler_ERP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Utilities;
using OfficeOpenXml;

namespace Exampler_ERP.Controllers.HR.MasterInfo
{
  public class ProcessTypeForwardController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ProcessTypeForwardController> _logger;
    private readonly Utils _utils;
    public ProcessTypeForwardController(AppDBContext appDBContext, IConfiguration configuration, ILogger<ProcessTypeForwardController> logger, Utils utils)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _logger = logger;
      _utils = utils;
    }
    public async Task<IActionResult> Index(string searchProcessTypeName)
    {
      // Query for Process Types with an optional search filter
      var processTypesQuery = _appDBContext.Settings_ProcessTypes.AsQueryable();

      if (!string.IsNullOrEmpty(searchProcessTypeName))
      {
        processTypesQuery = processTypesQuery
            .Where(pt => pt.ProcessTypeName.Contains(searchProcessTypeName));
      }

      var processTypes = await processTypesQuery.ToListAsync();
      var processTypeForwards = await _appDBContext.CR_ProcessTypeForwards.ToListAsync();

      // Create ViewModel based on query results
      var viewModel = new ProcessTypeForwardListViewModel
      {
        ProcessTypesWithRoleCount = processTypes.Select(pt => new ProcessTypeWithRoleCountViewModel
        {
          ProcessTypeID = pt.ProcessTypeID,
          ProcessTypeName = pt.ProcessTypeName,
          RoleCount = processTypeForwards.Count(ptf => ptf.ProcessTypeID == pt.ProcessTypeID)
        }).ToList()
      };

      return View("~/Views/HR/MasterInfo/ProcessTypeForward/ProcessTypeForward.cshtml", viewModel);
    }
    public async Task<IActionResult> Edit(int id)
    {
      var ProcessTypeForwards = await _appDBContext.CR_ProcessTypeForwards
       .Where(pt => pt.ProcessTypeID == id)
       .Include(pt => pt.ProcessType)
       .Include(pt => pt.RoleType)
       .ToListAsync();

      if (ProcessTypeForwards == null || !ProcessTypeForwards.Any())
      {
        var processType = await _appDBContext.Settings_ProcessTypes
            .Where(p => p.ProcessTypeID == id)
            .FirstOrDefaultAsync();

        if (processType != null)
        {
          ProcessTypeForwards = new List<CR_ProcessTypeForward>
            {
                new CR_ProcessTypeForward
                {
                    ProcessTypeID = processType.ProcessTypeID,
                    ProcessType = processType
                }
            };
        }
        else
        {
          return NotFound(); // Or handle accordingly if ProcessType is not found
        }
      }

      ViewBag.RoleList = await _appDBContext.Settings_RoleTypes
          .Select(r => new { Value = r.RoleTypeID, Text = r.RoleTypeName })
          .ToListAsync();

      ViewBag.ProcessTypes = await _appDBContext.Settings_ProcessTypes.ToListAsync();

      return PartialView("~/Views/HR/MasterInfo/ProcessTypeForward/EditProcessTypeForward.cshtml", ProcessTypeForwards);
    }



    [HttpPost]
    public async Task<IActionResult> Edit(List<CR_ProcessTypeForward> ProcessTypeForwards)
    {
      foreach (var setup in ProcessTypeForwards)
      {
        _logger.LogInformation("Received Setup: ID={ID}, RoleID={RoleID}", setup.ProcessTypeForwardID, setup.RoleTypeID);
      }

      if (ProcessTypeForwards == null || ProcessTypeForwards.Count == 0)
      {
        _logger.LogWarning("No data received for edit.");
        return Json(new { success = false, message = "No data received." });
      }

      if (ModelState.IsValid)
      {
        try
        {
          var processTypeId = ProcessTypeForwards.FirstOrDefault()?.ProcessTypeID;
          if (processTypeId == null)
          {
            return BadRequest("Invalid ProcessTypeID.");
          }

          var existingRecords = await _appDBContext.CR_ProcessTypeForwards
                                        .Where(p => p.ProcessTypeID == processTypeId)
                                        .ToListAsync();

          _appDBContext.CR_ProcessTypeForwards.RemoveRange(existingRecords);

          foreach (var setup in ProcessTypeForwards)
          {
            if (setup.RoleTypeID != 0)
            {
              _appDBContext.CR_ProcessTypeForwards.Add(setup);
            }
          }

          await _appDBContext.SaveChangesAsync();
          return Json(new { success = true });
        }
        catch (Exception ex)
        {
          _logger.LogError(ex, "Error updating ProcessTypeForwards");
          return Json(new { success = false, message = "An error occurred while updating the data." });
        }
      }

      var errors = ModelState.Values.SelectMany(v => v.Errors);
      return PartialView("~/Views/HR/MasterInfo/ProcessTypeForward/EditProcessTypeForward.cshtml", ProcessTypeForwards);
    }



    [HttpPost]
    public IActionResult Delete(int id)
    {
      var setup = _appDBContext.CR_ProcessTypeForwards.FirstOrDefault(x => x.ProcessTypeID == id);
      if (setup == null)
      {
        return Json(new { success = false, message = "Setup not found" });
      }

      _appDBContext.CR_ProcessTypeForwards.Remove(setup);
      _appDBContext.SaveChanges();

      return Json(new { success = true });
    }


    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var ProcessTypeForwards = await _appDBContext.CR_ProcessTypeForwards
      .Distinct()
      .Include(d => d.ProcessType) // Eagerly load the related Branch data
      .ToListAsync();


      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add("ProcessTypeForwards");
        worksheet.Cells["A1"].Value = "ProcessType ApprovalID";
        worksheet.Cells["B1"].Value = "ProcessType Name";
        worksheet.Cells["C1"].Value = "Role";
    

        for (int i = 0; i < ProcessTypeForwards.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = ProcessTypeForwards[i].ProcessTypeForwardID;
          worksheet.Cells[i + 2, 2].Value = ProcessTypeForwards[i].ProcessType?.ProcessTypeName;
          worksheet.Cells[i + 2, 3].Value = ProcessTypeForwards[i].RoleTypeID;
        }

        worksheet.Cells["A1:l1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = $"ProcessTypeForwards-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
    public async Task<IActionResult> Print()
    {
      var ProcessTypeForwards = await _appDBContext.CR_ProcessTypeForwards
      .Distinct()
      .Include(d => d.ProcessType) // Eagerly load the related Branch data
      .ToListAsync();
      return View("~/Views/HR/MasterInfo/ProcessTypeForward/PrintProcessTypeForwards.cshtml", ProcessTypeForwards);
    }

    //public async Task<IActionResult> Create()
    //{
    //  ViewBag.ProcessTypes = await _appDBContext.Settings_ProcessTypes.ToListAsync();
    //  ViewBag.Roles = await _appDBContext.Settings_RoleTypes.ToListAsync();
    //  return PartialView("~/Views/HR/MasterInfo/ProcessTypeForward/AddProcessTypeForward.cshtml");
    //}

    //[HttpPost]
    //public async Task<IActionResult> Create(ProcessTypeForwardViewModel model)
    //{
    //  if (ModelState.IsValid)
    //  {
    //    foreach (var roleId in model.SelectedRoleIds)
    //    {
    //      var processTypeForward = new CR_ProcessTypeForward
    //      {
    //        ProcessTypeID = model.ProcessTypeID,
    //        RoleTypeID = roleId
    //      };
    //      _appDBContext.CR_ProcessTypeForwards.Add(processTypeForward);
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

    //  ViewBag.ProcessTypes = await _appDBContext.Settings_ProcessTypes.ToListAsync();
    //  ViewBag.Roles = await _appDBContext.Settings_RoleTypes.ToListAsync();
    //  return PartialView("~/Views/HR/MasterInfo/ProcessTypeForward/AddProcessTypeForward.cshtml", model);
    //}

    //public async Task<IActionResult> Edit(int id)
    //{
    //  var processType = await _appDBContext.Settings_ProcessTypes.FindAsync(id);
    //  if (processType == null)
    //  {
    //    return NotFound();
    //  }

    //  var selectedRoleIds = await _appDBContext.CR_ProcessTypeForwards
    //      .Where(ptf => ptf.ProcessTypeID == id)
    //      .Select(ptf => ptf.RoleTypeID)
    //      .ToListAsync();

    //  var viewModel = new ProcessTypeForwardViewModel
    //  {
    //    ProcessTypeID = id,
    //    SelectedRoleIds = selectedRoleIds
    //  };

    //  ViewBag.ProcessTypes = await _appDBContext.Settings_ProcessTypes.ToListAsync();
    //  ViewBag.Roles = await _appDBContext.Settings_RoleTypes.ToListAsync();
    //  return PartialView("~/Views/HR/MasterInfo/ProcessTypeForward/EditProcessTypeForward.cshtml", viewModel);
    //}


    //[HttpPost]
    //public async Task<IActionResult> Edit(ProcessTypeForwardViewModel model)
    //{
    //  if (ModelState.IsValid)
    //  {
    //    // Remove existing entries
    //    var existingEntries = _appDBContext.CR_ProcessTypeForwards.Where(ptf => ptf.ProcessTypeID == model.ProcessTypeID);
    //    _appDBContext.CR_ProcessTypeForwards.RemoveRange(existingEntries);

    //    // Add new entries
    //    foreach (var roleId in model.SelectedRoleIds)
    //    {
    //      var processTypeForward = new CR_ProcessTypeForward
    //      {
    //        ProcessTypeID = model.ProcessTypeID,
    //        RoleTypeID = roleId
    //      };
    //      _appDBContext.CR_ProcessTypeForwards.Add(processTypeForward);
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

    //  ViewBag.ProcessTypes = await _appDBContext.Settings_ProcessTypes.ToListAsync();
    //  ViewBag.Roles = await _appDBContext.Settings_RoleTypes.ToListAsync();
    //  return PartialView("~/Views/HR/MasterInfo/ProcessTypeForward/EditProcessTypeForward.cshtml", model);
    //}
  }
}

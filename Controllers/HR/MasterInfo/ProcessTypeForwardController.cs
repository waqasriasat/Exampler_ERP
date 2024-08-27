using Exampler_ERP.Models.Temp;
using Exampler_ERP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Utilities;

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

    public async Task<IActionResult> Index()
    {
      var processTypes = await _appDBContext.Settings_ProcessTypes.ToListAsync();
      var processTypeForwards = await _appDBContext.CR_ProcessTypeForwards.ToListAsync();

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

    public async Task<IActionResult> Create()
    {
      ViewBag.ProcessTypes = await _appDBContext.Settings_ProcessTypes.ToListAsync();
      ViewBag.Roles = await _appDBContext.Settings_Roles.ToListAsync();
      return PartialView("~/Views/HR/MasterInfo/ProcessTypeForward/AddProcessTypeForward.cshtml");
    }

    [HttpPost]
    public async Task<IActionResult> Create(ProcessTypeForwardViewModel model)
    {
      if (ModelState.IsValid)
      {
        foreach (var roleId in model.SelectedRoleIds)
        {
          var processTypeForward = new CR_ProcessTypeForward
          {
            ProcessTypeID = model.ProcessTypeID,
            RoleID = roleId
          };
          _appDBContext.CR_ProcessTypeForwards.Add(processTypeForward);
        }
        await _appDBContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
      }

      // Log the validation errors
      foreach (var key in ModelState.Keys)
      {
        var state = ModelState[key];
        foreach (var error in state.Errors)
        {
          // Use your logging mechanism here
          Console.WriteLine($"Validation error in {key}: {error.ErrorMessage}");
        }
      }

      ViewBag.ProcessTypes = await _appDBContext.Settings_ProcessTypes.ToListAsync();
      ViewBag.Roles = await _appDBContext.Settings_Roles.ToListAsync();
      return PartialView("~/Views/HR/MasterInfo/ProcessTypeForward/AddProcessTypeForward.cshtml", model);
    }

    public async Task<IActionResult> Edit(int id)
    {
      var processType = await _appDBContext.Settings_ProcessTypes.FindAsync(id);
      if (processType == null)
      {
        return NotFound();
      }

      var selectedRoleIds = await _appDBContext.CR_ProcessTypeForwards
          .Where(ptf => ptf.ProcessTypeID == id)
          .Select(ptf => ptf.RoleID)
          .ToListAsync();

      var viewModel = new ProcessTypeForwardViewModel
      {
        ProcessTypeID = id,
        SelectedRoleIds = selectedRoleIds
      };

      ViewBag.ProcessTypes = await _appDBContext.Settings_ProcessTypes.ToListAsync();
      ViewBag.Roles = await _appDBContext.Settings_Roles.ToListAsync();
      return PartialView("~/Views/HR/MasterInfo/ProcessTypeForward/EditProcessTypeForward.cshtml", viewModel);
    }

   
    [HttpPost]
    public async Task<IActionResult> Edit(ProcessTypeForwardViewModel model)
    {
      if (ModelState.IsValid)
      {
        // Remove existing entries
        var existingEntries = _appDBContext.CR_ProcessTypeForwards.Where(ptf => ptf.ProcessTypeID == model.ProcessTypeID);
        _appDBContext.CR_ProcessTypeForwards.RemoveRange(existingEntries);

        // Add new entries
        foreach (var roleId in model.SelectedRoleIds)
        {
          var processTypeForward = new CR_ProcessTypeForward
          {
            ProcessTypeID = model.ProcessTypeID,
            RoleID = roleId
          };
          _appDBContext.CR_ProcessTypeForwards.Add(processTypeForward);
        }

        await _appDBContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
      }

      // Log the validation errors
      foreach (var key in ModelState.Keys)
      {
        var state = ModelState[key];
        foreach (var error in state.Errors)
        {
          _logger.LogError($"Validation error in {key}: {error.ErrorMessage}");
        }
      }

      ViewBag.ProcessTypes = await _appDBContext.Settings_ProcessTypes.ToListAsync();
      ViewBag.Roles = await _appDBContext.Settings_Roles.ToListAsync();
      return PartialView("~/Views/HR/MasterInfo/ProcessTypeForward/EditProcessTypeForward.cshtml", model);
    }
  }
}

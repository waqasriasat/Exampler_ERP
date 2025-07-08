using Exampler_ERP.Models;
using Exampler_ERP.Models.Temp;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml;
using System.Data;

namespace Exampler_ERP.Controllers.HR.MasterInfo
{
  public class ProcessTypeApprovalSetupController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ProcessTypeApprovalSetupController> _logger;
    private readonly Utils _utils;
private readonly IHubContext<NotificationHub> _hubContext;

    public ProcessTypeApprovalSetupController(AppDBContext appDBContext, IConfiguration configuration, ILogger<ProcessTypeApprovalSetupController> logger, Utils utils, IHubContext<NotificationHub> hubContext)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _logger = logger;
      _utils = utils;
_hubContext = hubContext;
 
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
      var processTypeApprovalSetup = await _appDBContext.CR_ProcessTypeApprovalSetups.ToListAsync();

      // Create ViewModel based on query results
      var viewModel = new ProcessTypeApprovalSetupListViewModel
      {
        ProcessTypesWithRankCount = processTypes.Select(pt => new ProcessTypeWithRankCountViewModel
        {
          ProcessTypeID = pt.ProcessTypeID,
          ProcessTypeName = pt.ProcessTypeName,
          RankCount = processTypeApprovalSetup.Count(ptf => ptf.ProcessTypeID == pt.ProcessTypeID)
        }).ToList()
      };

      if (!string.IsNullOrEmpty(searchProcessTypeName) && viewModel.ProcessTypesWithRankCount.Count == 0)
      {
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "No Process Type found with the name '" + searchProcessTypeName + "'. Please check the name and try again.");
      }

      return View("~/Views/HR/MasterInfo/ProcessTypeApprovalSetup/ProcessTypeApprovalSetup.cshtml", viewModel);
    }

   
    public async Task<IActionResult> ProcessTypeApprovalSetup()
    {
      var ProcessTypeApprovalSetups = await _appDBContext.CR_ProcessTypeApprovalSetups.ToListAsync();
      return Ok(ProcessTypeApprovalSetups);
    }
    public async Task<IActionResult> Edit(int id)
    {
      var processTypeApprovalSetups = await _appDBContext.CR_ProcessTypeApprovalSetups
       .Where(pt => pt.ProcessTypeID == id)
       .Include(pt => pt.ProcessType)
       .Include(pt => pt.RoleType)
       .ToListAsync();

      if (processTypeApprovalSetups == null || !processTypeApprovalSetups.Any())
      {
        var processType = await _appDBContext.Settings_ProcessTypes
            .Where(p => p.ProcessTypeID == id)
            .FirstOrDefaultAsync();

        if (processType != null)
        {
          processTypeApprovalSetups = new List<CR_ProcessTypeApprovalSetup>
            {
                new CR_ProcessTypeApprovalSetup
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

      return PartialView("~/Views/HR/MasterInfo/ProcessTypeApprovalSetup/EditProcessTypeApprovalSetup.cshtml", processTypeApprovalSetups);
    }

   

    [HttpPost]
    public async Task<IActionResult> Edit(List<CR_ProcessTypeApprovalSetup> ProcessTypeApprovalSetups)
    {
      foreach (var setup in ProcessTypeApprovalSetups)
      {
        _logger.LogInformation("Received Setup: ID={ID}, Rank={Rank}, RoleID={RoleID}", setup.ProcessTypeApprovalSetupID, setup.Rank, setup.RoleTypeID);
      }

      if (ProcessTypeApprovalSetups == null || ProcessTypeApprovalSetups.Count == 0)
      {
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "No data received.");
        _logger.LogWarning("No data received for edit.");
        return Json(new { success = false, message = "No data received." });
      }

      if (ModelState.IsValid)
      {
        try
        {
          var processTypeId = ProcessTypeApprovalSetups.FirstOrDefault()?.ProcessTypeID;
          if (processTypeId == null)
          {
            await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "Invalid ProcessTypeID.");
            return BadRequest("Invalid ProcessTypeID.");
          }

          var existingRecords = await _appDBContext.CR_ProcessTypeApprovalSetups
                                        .Where(p => p.ProcessTypeID == processTypeId)
                                        .ToListAsync();

          _appDBContext.CR_ProcessTypeApprovalSetups.RemoveRange(existingRecords);

          foreach (var setup in ProcessTypeApprovalSetups)
          {
            if (setup.RoleTypeID != 0 && setup.Rank != 0)
            {
              _appDBContext.CR_ProcessTypeApprovalSetups.Add(setup);
            }
          }

          await _appDBContext.SaveChangesAsync();
          await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Process Type Approval Setup Update successfully.");
          return Json(new { success = true });
        }
        catch (Exception ex)
        {
          await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "An error occurred while updating the data.");
          _logger.LogError(ex, "Error updating ProcessTypeApprovalSetups");
          return Json(new { success = false, message = "An error occurred while updating the data." });
        }
      }

      var errors = ModelState.Values.SelectMany(v => v.Errors);
      return PartialView("~/Views/HR/MasterInfo/ProcessTypeApprovalSetup/EditProcessTypeApprovalSetup.cshtml", ProcessTypeApprovalSetups);
    }



    [HttpPost]
    public IActionResult Delete(int id)
    {
      var setups = _appDBContext.CR_ProcessTypeApprovalSetups
        .Where(x => x.ProcessTypeID == id)
        .ToList();
      if (setups == null)
      {
        return Json(new { success = false, message = "Setup not found" });
      }

      _appDBContext.CR_ProcessTypeApprovalSetups.RemoveRange(setups);
      _appDBContext.SaveChanges();
       _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Process Type Approval Setups deleted successfully.");
      return Json(new { success = true });
    }


    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var ProcessTypeApprovalSetups = await _appDBContext.CR_ProcessTypeApprovalSetups
      .Distinct()
      .Include(d => d.ProcessType) // Eagerly load the related Branch data
      .ToListAsync();


      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add("ProcessTypeApprovalSetups");
        worksheet.Cells["A1"].Value = "ProcessType SetupID";
        worksheet.Cells["B1"].Value = "ProcessType Name";
        worksheet.Cells["C1"].Value = "Rank";
        worksheet.Cells["D1"].Value = "Role";


        for (int i = 0; i < ProcessTypeApprovalSetups.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = ProcessTypeApprovalSetups[i].ProcessTypeApprovalSetupID;
          worksheet.Cells[i + 2, 2].Value = ProcessTypeApprovalSetups[i].ProcessType?.ProcessTypeName;
          worksheet.Cells[i + 2, 3].Value = ProcessTypeApprovalSetups[i].Rank;
          worksheet.Cells[i + 2, 4].Value = ProcessTypeApprovalSetups[i].RoleTypeID;
        }

        worksheet.Cells["A1:l1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = $"ProcessTypeApprovalSetups-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
    public async Task<IActionResult> Print()
    {
      var ProcessTypeApprovalSetups = await _appDBContext.CR_ProcessTypeApprovalSetups
      .Distinct()
      .Include(d => d.ProcessType) // Eagerly load the related Branch data
      .ToListAsync();
      return View("~/Views/HR/MasterInfo/ProcessTypeApprovalSetup/PrintProcessTypeApprovalSetups.cshtml", ProcessTypeApprovalSetups);
    }
    
  }
}

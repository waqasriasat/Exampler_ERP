using Exampler_ERP.Models.Temp;
using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Exampler_ERP.Controllers.MasterInfo
{
  public class ApprovalsEmployeeRequestController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
private readonly IHubContext<NotificationHub> _hubContext;

    public ApprovalsEmployeeRequestController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
_hubContext = hubContext;
 
    }

    public async Task<IActionResult> Index(DateTime? FromDate, DateTime? ToDate, string? EmployeeName, int? EmployeeID, int? EmployeeRequestTypeID)
    {
      var query = _appDBContext.HR_EmployeeRequestTypeApprovalDetails
          .Include(pta => pta.HR_EmployeeRequestTypeApproval)
              .ThenInclude(pta => pta.Employee)
          .Include(pta => pta.HR_EmployeeRequestTypeApproval)
              .ThenInclude(pta => pta.EmployeeRequestType)
          .Include(pta => pta.EmployeeRequestTypeApprovalDetailDoc)
          .Where(pta => pta.AppID == 0);

      if (FromDate.HasValue)
      {
        var fromDateTime = FromDate.Value.Date.AddSeconds(1);
        query = query.Where(pta => pta.HR_EmployeeRequestTypeApproval.Date >= fromDateTime);
      }

      if (ToDate.HasValue)
      {
        var toDateTime = ToDate.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
        query = query.Where(pta => pta.HR_EmployeeRequestTypeApproval.Date <= toDateTime);
      }

      // Filter by EmployeeName
      if (EmployeeID.HasValue)
      {
        query = query.Where(pta => pta.HR_EmployeeRequestTypeApproval.EmployeeID <= EmployeeID.Value);
      }

      // Filter by EmployeeRequestTypeID
      if (EmployeeRequestTypeID.HasValue && EmployeeRequestTypeID != 0)
      {
        query = query.Where(pta => pta.HR_EmployeeRequestTypeApproval.EmployeeRequestTypeID == EmployeeRequestTypeID.Value);
      }

      ViewBag.FromDate = FromDate;
      ViewBag.ToDate = ToDate;
      ViewBag.EmployeeRequestTypeID = EmployeeRequestTypeID;
      ViewBag.EmployeeID = EmployeeID;
      ViewBag.EmployeeName = EmployeeName;

      var result = await query
          .OrderByDescending(pta => pta.EmployeeRequestTypeApprovalDetailID)
          .ToListAsync();

      ViewBag.EmployeeRequestTypeList = await _utils.GetEmployeeRequestTypes();
      return View("~/Views/MasterInfo/ApprovalsEmployeeRequest/ApprovalsEmployeeRequestSearching.cshtml", result);
    }
    public async Task<IActionResult> SelectedIndex(int? id = null)
    {
      var query = _appDBContext.HR_EmployeeRequestTypeApprovalDetails
          .Include(pta => pta.HR_EmployeeRequestTypeApproval)
              .ThenInclude(pta => pta.Employee)
          .Include(pta => pta.HR_EmployeeRequestTypeApproval)
              .ThenInclude(pta => pta.EmployeeRequestType)
          .Include(pta => pta.EmployeeRequestTypeApprovalDetailDoc)
          .Where(pta => pta.AppID == 0);

      if (id.HasValue)
      {
        query = query.Where(pta => pta.HR_EmployeeRequestTypeApproval.EmployeeRequestTypeID == id.Value);
      }

      var result = await query
          .OrderByDescending(pta => pta.EmployeeRequestTypeApprovalDetailID)
          .ToListAsync();

      ViewBag.EmployeeRequestTypeList = await _utils.GetEmployeeRequestTypes();
      return View("~/Views/MasterInfo/ApprovalsEmployeeRequest/ApprovalsEmployeeRequest.cshtml", result);
    }
    [HttpGet]
    public async Task<IActionResult> DownloadDocument(int id)
    {
      var document = await _appDBContext.HR_EmployeeRequestTypeApprovalDetailDocs
          .FirstOrDefaultAsync(d => d.EmployeeRequestTypeApprovalDetailDocID == id);

      if (document == null || document.Doc == null)
      {
        return NotFound();
      }

      return File(document.Doc, "application/octet-stream", document.DocName);
    }
    [HttpGet]
    public async Task<IActionResult> ViewDocument(int id)
    {
      // Retrieve the document based on EmployeeRequestTypeApprovalDetailDocID
      var document = await _appDBContext.HR_EmployeeRequestTypeApprovalDetailDocs
          .FirstOrDefaultAsync(d => d.EmployeeRequestTypeApprovalDetailDocID == id);

      if (document == null || document.Doc == null)
      {
        // If document is not found, return a 404 error.
        return NotFound();
      }

      // Determine MIME type based on file extension
      string mimeType = document.DocExt.ToLower() switch
      {
        ".pdf" => "application/pdf",
        ".jpg" => "image/jpeg",
        ".jpeg" => "image/jpeg",
        ".png" => "image/png",
        _ => "application/octet-stream"
      };

      // Return the document for inline viewing
      return File(document.Doc, mimeType, document.DocName);
    }
    public async Task<IActionResult> Approvals(int id)
    {


      var result = await _appDBContext.HR_EmployeeRequestTypeApprovalDetails
    .Include(pta => pta.HR_EmployeeRequestTypeApproval)
    .ThenInclude(pta => pta.Employee)
    .Include(pta => pta.HR_EmployeeRequestTypeApproval)
    .ThenInclude(pta => pta.EmployeeRequestType)
    .Include(pta => pta.EmployeeRequestTypeApprovalDetailDoc)
    .Where(pta => pta.AppID == 1 && pta.EmployeeRequestTypeApprovalID == id)
    .OrderBy(pta => pta.Rank)
    .ToListAsync();

      ViewBag.EmployeesList = await _utils.GetEmployee();
      ViewBag.RolesList = await _utils.GetRoles();


      if (result == null)
      {
        return NotFound($"No approval record found for EmployeeRequestTypeApprovalID = {id}");
      }

      return PartialView("~/Views/MasterInfo/ApprovalsEmployeeRequest/ApprovalsEmployeeRequestTypeApproval.cshtml", result);
    }
    public async Task<IActionResult> Actions(int id)
    {
      var result = await _appDBContext.HR_EmployeeRequestTypeApprovalDetails
               .Where(pta => pta.EmployeeRequestTypeApprovalDetailID == id)
               .Include(pta => pta.HR_EmployeeRequestTypeApproval)
               .FirstOrDefaultAsync();

      var EmployeeRequestTypeID = result?.HR_EmployeeRequestTypeApproval?.EmployeeRequestTypeID;
      var RankID = result?.Rank;
      var EmployeeRequestTypeApprovalDetailID = result?.EmployeeRequestTypeApprovalDetailID;

      ViewBag.EmployeeRequestTypeID = EmployeeRequestTypeID;
      ViewBag.RankID = RankID;
      ViewBag.EmployeeRequestTypeApprovalDetailID = EmployeeRequestTypeApprovalDetailID;


      if (result == null)
      {
        return NotFound($"No approval record found for EmployeeRequestTypeApprovalID = {id}");
      }

      return PartialView("~/Views/MasterInfo/ApprovalsEmployeeRequest/ActionsEmployeeRequestTypeApproval.cshtml", result);
    }
    [HttpPost]
    public async Task<IActionResult> Approved(HR_EmployeeRequestTypeApprovalDetail EmployeeRequestTypeApprovalDetail, IFormFile FileUpload, int EmployeeRequestTypeID)
    {
      if (ModelState.IsValid)
      {
        EmployeeRequestTypeApprovalDetail.AppID = 1;
        EmployeeRequestTypeApprovalDetail.AppUserID = HttpContext.Session.GetInt32("UserID") ?? default(int);
        EmployeeRequestTypeApprovalDetail.Date = DateTime.Now;

        if (FileUpload != null && FileUpload.Length > 0)
        {
          using (var memoryStream = new MemoryStream())
          {
            await FileUpload.CopyToAsync(memoryStream);

            var EmployeeRequestTypeApprovalDetailDoc = new HR_EmployeeRequestTypeApprovalDetailDoc
            {
              EmployeeRequestTypeApprovalDetailID = EmployeeRequestTypeApprovalDetail.EmployeeRequestTypeApprovalDetailID,
              Doc = memoryStream.ToArray(),
              DocName = FileUpload.FileName,
              DocExt = Path.GetExtension(FileUpload.FileName)
            };

            EmployeeRequestTypeApprovalDetail.EmployeeRequestTypeApprovalDetailDoc.Add(EmployeeRequestTypeApprovalDetailDoc);
          }
        }

        _appDBContext.Update(EmployeeRequestTypeApprovalDetail);
        await _appDBContext.SaveChangesAsync();

        var EmployeeRequestTypeId = EmployeeRequestTypeID;
        var currentRank = EmployeeRequestTypeApprovalDetail.Rank;

        var nextApprovalSetup = await _appDBContext.HR_EmployeeRequestTypeApprovalSetups
            .Where(pta => pta.EmployeeRequestTypeID == EmployeeRequestTypeId && pta.Rank == currentRank + 1)
            .FirstOrDefaultAsync();

        if (nextApprovalSetup != null)
        {
          var newApprovalSetup = new HR_EmployeeRequestTypeApprovalDetail
          {
            EmployeeRequestTypeApprovalID = EmployeeRequestTypeApprovalDetail.EmployeeRequestTypeApprovalID,
            Date = EmployeeRequestTypeApprovalDetail.Date,
            RoleID = nextApprovalSetup.RoleTypeID,
            AppID = 0,
            AppUserID = 0,
            Notes = null,
            Rank = nextApprovalSetup.Rank
          };

          _appDBContext.HR_EmployeeRequestTypeApprovalDetails.Add(newApprovalSetup);
          await _appDBContext.SaveChangesAsync();
        }
        else
        {
            var EmployeeRequestTypeApproval = await _appDBContext.HR_EmployeeRequestTypeApprovals
            .Where(u => u.EmployeeRequestTypeApprovalID == EmployeeRequestTypeApprovalDetail.EmployeeRequestTypeApprovalID)
            .FirstOrDefaultAsync();

            if (EmployeeRequestTypeApproval != null)
            {
            EmployeeRequestTypeApproval.FinalApprovalID = 1;
       
              _appDBContext.Update(EmployeeRequestTypeApproval);
              await _appDBContext.SaveChangesAsync();
            }
        }
        return Json(new { success = true });
      }

      return PartialView("~/Views/MasterInfo/ApprovalsEmployeeRequest/ActionsEmployeeRequestTypeApproval.cshtml", EmployeeRequestTypeApprovalDetail);
    }
    [HttpPost]
    public async Task<IActionResult> Reject(HR_EmployeeRequestTypeApprovalDetail EmployeeRequestTypeApprovalDetail, IFormFile FileUpload, int EmployeeRequestTypeID)
    {
      if (ModelState.IsValid)
      {
        EmployeeRequestTypeApprovalDetail.AppID = 1;
        EmployeeRequestTypeApprovalDetail.AppUserID = HttpContext.Session.GetInt32("UserID") ?? default(int);
        EmployeeRequestTypeApprovalDetail.Date = DateTime.Now;

        if (FileUpload != null && FileUpload.Length > 0)
        {
          using (var memoryStream = new MemoryStream())
          {
            await FileUpload.CopyToAsync(memoryStream);

            var EmployeeRequestTypeApprovalDetailDoc = new HR_EmployeeRequestTypeApprovalDetailDoc
            {
              EmployeeRequestTypeApprovalDetailID = EmployeeRequestTypeApprovalDetail.EmployeeRequestTypeApprovalDetailID,
              Doc = memoryStream.ToArray(),
              DocName = FileUpload.FileName,
              DocExt = Path.GetExtension(FileUpload.FileName)
            };

            EmployeeRequestTypeApprovalDetail.EmployeeRequestTypeApprovalDetailDoc.Add(EmployeeRequestTypeApprovalDetailDoc);
          }
        }

        _appDBContext.Update(EmployeeRequestTypeApprovalDetail);
        await _appDBContext.SaveChangesAsync();

        var EmployeeRequestTypeId = EmployeeRequestTypeID;
        var EmployeeRequestTypeApproval = await _appDBContext.HR_EmployeeRequestTypeApprovals
           .Where(u => u.EmployeeRequestTypeApprovalID == EmployeeRequestTypeApprovalDetail.EmployeeRequestTypeApprovalID)
           .FirstOrDefaultAsync();

        if (EmployeeRequestTypeApproval != null)
        {
          EmployeeRequestTypeApproval.FinalApprovalID = 2;

          _appDBContext.Update(EmployeeRequestTypeApproval);
          await _appDBContext.SaveChangesAsync();
        }
        return Json(new { success = true });
      }

      return PartialView("~/Views/MasterInfo/ApprovalsEmployeeRequest/ActionsEmployeeRequestTypeApproval.cshtml", EmployeeRequestTypeApprovalDetail);
    }
    public async Task<IActionResult> Details(int id)
    {
      // Fetch the EmployeeRequestTypeApproval record
      var fatchDetail = await _appDBContext.HR_EmployeeRequestTypeApprovals
          .Where(pta => pta.EmployeeRequestTypeApprovalID == id)
          .FirstOrDefaultAsync();

      if (fatchDetail == null)
      {
        return NotFound();
      }
      var EmployeeData = await _appDBContext.HR_Employees
          .Where(u => u.EmployeeID == fatchDetail.EmployeeID)
          .FirstOrDefaultAsync();
      var EmployeeName = EmployeeData?.FirstName+' '+ EmployeeData?.FatherName + ' ' + EmployeeData?.FamilyName;
      HttpContext.Session.SetString("EmployeeName", EmployeeName);

      var EmployeeRequestTypeID = fatchDetail.EmployeeRequestTypeID;
 
      ViewBag.EmployeeRequestTypeID = EmployeeRequestTypeID;
  
      object result = null;

      if (EmployeeRequestTypeID >0)
      {
        ViewBag.EmployeeRequestTypeList = await _utils.GetEmployeeRequestTypes();
        var EmployeeRequests = await _appDBContext.HR_EmployeeRequestTypeApprovals
        .Where(v => v.EmployeeRequestTypeApprovalID == id)
                                     .Include(c => c.Employee)
                                     .Include(c => c.EmployeeRequestType)
                                     .FirstOrDefaultAsync();

        return PartialView("~/Views/MasterInfo/ApprovalsEmployeeRequest/DetailsEmployeeRequestTypeApproval.cshtml", EmployeeRequests);
      }
      return View("~/Views/MasterInfo/ApprovalsEmployeeRequest/ApprovalsEmployeeRequest.cshtml", result);
    }
  }
}

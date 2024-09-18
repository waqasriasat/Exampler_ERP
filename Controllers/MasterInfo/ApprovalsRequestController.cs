using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Exampler_ERP.Controllers.MasterInfo
{
  public class ApprovalsRequestController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    public ApprovalsRequestController(AppDBContext appDBContext, IConfiguration configuration, Utils utils)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
    }
    public async Task<IActionResult> Index()
    {
      var result = await _appDBContext.CR_ProcessTypeApprovalDetails
    .Include(pta => pta.CR_ProcessTypeApproval)
    .ThenInclude(pta => pta.Employee)
    .Include(pta => pta.CR_ProcessTypeApproval)
    .ThenInclude(pta => pta.ProcessType)
    .Include(pta => pta.ProcessTypeApprovalDetailDoc)
    .Where(pta => pta.AppID == 0)
    .OrderByDescending(pta => pta.ApprovalProcessDetailID)
    .ToListAsync();

      return View("~/Views/MasterInfo/ApprovalsRequest/ApprovalsRequest.cshtml", result);
    }
    public async Task<IActionResult> Approvals(int id)
    {
    

      var result = await _appDBContext.CR_ProcessTypeApprovalDetails
    .Include(pta => pta.CR_ProcessTypeApproval) 
    .ThenInclude(pta => pta.Employee)
    .Include(pta => pta.CR_ProcessTypeApproval) 
    .ThenInclude(pta => pta.ProcessType)
    .Include(pta => pta.ProcessTypeApprovalDetailDoc) 
    .Where(pta => pta.AppID == 1 && pta.ApprovalProcessID == id)
    .OrderBy(pta => pta.Rank) 
    .ToListAsync();

      ViewBag.EmployeesList = await _utils.GetEmployee();
      ViewBag.RolesList = await _utils.GetRoles();


      if (result == null)
      {
        return NotFound($"No approval record found for ApprovalProcessID = {id}");
      }

      return PartialView("~/Views/MasterInfo/ApprovalsRequest/ApprovalsProcessTypeApproval.cshtml", result);
    }
    public async Task<IActionResult> Actions(int id)
    {
      var result = await _appDBContext.CR_ProcessTypeApprovalDetails
               .Where(pta => pta.ApprovalProcessDetailID == id)
               .Include(pta => pta.CR_ProcessTypeApproval)
               .FirstOrDefaultAsync();

      var ProcessTypeID = result?.CR_ProcessTypeApproval?.ProcessTypeID;
      var TransactionID = result?.CR_ProcessTypeApproval?.TransactionID;
      var RankID = result?.Rank;
      var ApprovalProcessDetailID = result?.ApprovalProcessDetailID;

      ViewBag.ProcessTypeID = ProcessTypeID;
      ViewBag.TransactionID = TransactionID;
      ViewBag.RankID = RankID;
      ViewBag.ApprovalProcessDetailID = ApprovalProcessDetailID;


      if (result == null)
      {
        return NotFound($"No approval record found for ApprovalProcessID = {id}");
      }

      return PartialView("~/Views/MasterInfo/ApprovalsRequest/ActionsProcessTypeApproval.cshtml", result);
    }

    [HttpPost]
    public async Task<IActionResult> Approved(CR_ProcessTypeApprovalDetail processTypeApprovalDetail, IFormFile FileUpload, int ProcessTypeID, int TransactionID)
    {
      if (ModelState.IsValid)
      {
        processTypeApprovalDetail.AppID = 1;
        processTypeApprovalDetail.AppUserID = HttpContext.Session.GetInt32("UserID") ?? default(int);
        processTypeApprovalDetail.Date = DateTime.Now;

        if (FileUpload != null && FileUpload.Length > 0)
        {
          using (var memoryStream = new MemoryStream())
          {
            await FileUpload.CopyToAsync(memoryStream);

            var processTypeApprovalDetailDoc = new CR_ProcessTypeApprovalDetailDoc
            {
              ApprovalProcessDetailID = processTypeApprovalDetail.ApprovalProcessDetailID,
              Doc = memoryStream.ToArray(),
              DocName = FileUpload.FileName,
              DocExt = Path.GetExtension(FileUpload.FileName)
            };

            processTypeApprovalDetail.ProcessTypeApprovalDetailDoc.Add(processTypeApprovalDetailDoc);
          }
        }

        _appDBContext.Update(processTypeApprovalDetail);
        await _appDBContext.SaveChangesAsync();

        var processTypeId = ProcessTypeID;
        var transactionID = TransactionID;
        var currentRank = processTypeApprovalDetail.Rank;

        var nextApprovalSetup = await _appDBContext.CR_ProcessTypeApprovalSetups
            .Where(pta => pta.ProcessTypeID == processTypeId && pta.Rank == currentRank + 1)
            .FirstOrDefaultAsync();

        if (nextApprovalSetup != null)
        {
          var newApprovalSetup = new CR_ProcessTypeApprovalDetail
          {
            ApprovalProcessID = processTypeApprovalDetail.ApprovalProcessID,
            Date = processTypeApprovalDetail.Date,
            RoleID = nextApprovalSetup.RoleID,
            AppID = 0,
            AppUserID = 0,
            Notes = null,
            Rank = nextApprovalSetup.Rank
          };

          _appDBContext.CR_ProcessTypeApprovalDetails.Add(newApprovalSetup);
          await _appDBContext.SaveChangesAsync();
        }
        else
        {
          if (ProcessTypeID == 1)
          {
            var user = await _appDBContext.CR_Users
            .Where(u => u.UserID == transactionID)
            .FirstOrDefaultAsync();

            if (user != null)
            {
              user.ActiveYNID = 1;
              user.FinalApprovalID = 1;
              user.ApprovalProcessID = processTypeApprovalDetail.ApprovalProcessID;

              _appDBContext.Update(user);
              await _appDBContext.SaveChangesAsync();
            }
          }
        }
        return Json(new { success = true });
      }

      return PartialView("~/Views/MasterInfo/ApprovalsRequest/ActionsProcessTypeApproval.cshtml", processTypeApprovalDetail);
    }
    [HttpPost]
    public async Task<IActionResult> Reject(CR_ProcessTypeApprovalDetail processTypeApprovalDetail, IFormFile FileUpload, int ProcessTypeID, int TransactionID)
    {
      if (ModelState.IsValid)
      {
        processTypeApprovalDetail.AppID = 1;
        processTypeApprovalDetail.AppUserID = HttpContext.Session.GetInt32("UserID") ?? default(int);
        processTypeApprovalDetail.Date = DateTime.Now;

        if (FileUpload != null && FileUpload.Length > 0)
        {
          using (var memoryStream = new MemoryStream())
          {
            await FileUpload.CopyToAsync(memoryStream);

            var processTypeApprovalDetailDoc = new CR_ProcessTypeApprovalDetailDoc
            {
              ApprovalProcessDetailID = processTypeApprovalDetail.ApprovalProcessDetailID,
              Doc = memoryStream.ToArray(),
              DocName = FileUpload.FileName,
              DocExt = Path.GetExtension(FileUpload.FileName)
            };

            processTypeApprovalDetail.ProcessTypeApprovalDetailDoc.Add(processTypeApprovalDetailDoc);
          }
        }

        _appDBContext.Update(processTypeApprovalDetail);
        await _appDBContext.SaveChangesAsync();

        var processTypeId = ProcessTypeID;
        var transactionID = TransactionID;
          if (ProcessTypeID == 1)
          {
            var user = await _appDBContext.CR_Users
            .Where(u => u.UserID == transactionID)
            .FirstOrDefaultAsync();

            if (user != null)
            {
              user.ActiveYNID = 0;
              user.FinalApprovalID = 2;
              user.ApprovalProcessID = processTypeApprovalDetail.ApprovalProcessID;

              _appDBContext.Update(user);
              await _appDBContext.SaveChangesAsync();
            }
        }
        return Json(new { success = true });
      }

      return PartialView("~/Views/MasterInfo/ApprovalsRequest/ActionsProcessTypeApproval.cshtml", processTypeApprovalDetail);
    }
    public async Task<IActionResult> Details(int id)
    {
      var FatchDetail = await _appDBContext.CR_ProcessTypeApprovals
          .Where(pta => pta.ApprovalProcessID == id)
          .FirstOrDefaultAsync();

      var processTypeID = FatchDetail?.ProcessTypeID;
      var transactionID = FatchDetail?.TransactionID;

      ViewBag.ProcessTypeID = processTypeID;
      ViewBag.TransactionID = transactionID;

      object result = null;

      if (processTypeID == 1)
      {
        ViewBag.RoleList = await _utils.GetRoles();
        ViewBag.EmployeeList = await _utils.GetEmployee();
        var user = await _appDBContext.CR_Users.FindAsync(transactionID);
        if (user == null)
        {
          return NotFound();
        }
        user.UserName = CR_CipherKey.Decrypt(user.UserName);
        result = user; 
      }
      else
      {
        return NotFound($"No approval record found for ApprovalProcessID = {id}");
      }

      if (result == null)
      {
        return NotFound($"No approval record found for ApprovalProcessID = {id}");
      }

      if (result is Exampler_ERP.Models.CR_User)
      {
        return PartialView("~/Views/MasterInfo/ApprovalsRequest/DetailsProcessTypeApproval.cshtml", result);
      }

      return View("~/Views/MasterInfo/ApprovalsRequest/ApprovalsRequest.cshtml", result);
    }


  }
}

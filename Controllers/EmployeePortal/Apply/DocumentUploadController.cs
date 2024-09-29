using Exampler_ERP.Models;
using Exampler_ERP.Models.Temp;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace Exampler_ERP.Controllers.EmployeePortal.Apply
{
  public class DocumentUploadController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    public DocumentUploadController(AppDBContext appDBContext, IConfiguration configuration, Utils utils)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
    }
    public async Task<IActionResult> Index()
    {
      var employeeID = HttpContext.Session.GetInt32("EmployeeID");

      var DocumentUploads = await _appDBContext.HR_DocumentUploads
                                   .Where(v => employeeID != null && v.EmployeeID == employeeID && v.DeleteYNID != 1)
                                   .OrderByDescending(v => v.DocumentUploadID)
                                   .Include(c => c.Employee)
                                   .ToListAsync();

      return View("~/Views/EmployeePortal/Apply/DocumentUpload/DocumentUpload.cshtml", DocumentUploads);
    }
    public async Task<IActionResult> Edit(int id)
    {
      var DocumentUploads = await _appDBContext.HR_DocumentUploads
      .Where(v => v.DocumentUploadID == id)
                                   .Include(c => c.Employee)
                                   .FirstOrDefaultAsync();

      return PartialView("~/Views/EmployeePortal/Apply/DocumentUpload/EditDocumentUpload.cshtml", DocumentUploads);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(HR_DocumentUpload documentUpload, IFormFile? DocumentData)
    {
      if (ModelState.IsValid)
      {
        var existingDocument = await _appDBContext.HR_DocumentUploads.FindAsync(documentUpload.DocumentUploadID);

        if (existingDocument != null)
        {
          existingDocument.DocumentName = documentUpload.DocumentName;
          existingDocument.DocumentType = documentUpload.DocumentType;
          existingDocument.Description = documentUpload.Description;

          // Update the document data only if a new file is uploaded
          if (DocumentData != null && DocumentData.Length > 0)
          {
            using (var memoryStream = new MemoryStream())
            {
              await DocumentData.CopyToAsync(memoryStream);
              existingDocument.DocumentData = memoryStream.ToArray(); // Update with new document data
            }
          }

          await _appDBContext.SaveChangesAsync();
          return Json(new { success = true, message = "Document updated successfully!" });
        }

        return Json(new { success = false, message = "Document not found." });
      }
      return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
      return PartialView("~/Views/EmployeePortal/Apply/DocumentUpload/AddDocumentUpload.cshtml", new HR_DocumentUpload());
    }

    [HttpPost]
    public async Task<IActionResult> Create(HR_DocumentUpload DocumentUpload, IFormFile DocumentData)
    {
     
      if (ModelState.IsValid)
      {
        if (DocumentData != null && DocumentData.Length > 0)
        {
          using (var memoryStream = new MemoryStream())
          {
            await DocumentData.CopyToAsync(memoryStream);
            DocumentUpload.DocumentData = memoryStream.ToArray();
          }

        }
        DocumentUpload.Date = DateTime.Now;
        _appDBContext.HR_DocumentUploads.Add(DocumentUpload);
        await _appDBContext.SaveChangesAsync();
        return Json(new { success = true });
      }
      return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
    }


    public async Task<IActionResult> Delete(int id)
    {
      var DocumentUploads = await _appDBContext.HR_DocumentUploads.FindAsync(id);
      if (DocumentUploads == null)
      {
        return NotFound();
      }

      DocumentUploads.DeleteYNID = 1;

      _appDBContext.HR_DocumentUploads.Update(DocumentUploads);
      await _appDBContext.SaveChangesAsync();

      return Json(new { success = true });
    }

    public async Task<IActionResult> Print()
    {
      var employeeID = HttpContext.Session.GetInt32("EmployeeID");

      var DocumentUploads = await _appDBContext.HR_DocumentUploads
                                   .Where(v => employeeID != null && v.EmployeeID == employeeID && v.DeleteYNID != 1)
                                   .OrderByDescending(v => v.DocumentUploadID)
                                   .Include(c => c.Employee)
                                   .ToListAsync();

      return View("~/Views/EmployeePortal/Apply/DocumentUpload/PrintDocumentUpload.cshtml", DocumentUploads);
    }
    [HttpGet]
    public async Task<IActionResult> DownloadDocument(int id)
    {
      var document = await _appDBContext.HR_DocumentUploads
          .FirstOrDefaultAsync(d => d.DocumentUploadID == id);

      if (document == null || document.DocumentData == null)
      {
        return NotFound();
      }

      return File(document.DocumentData, "application/octet-stream", document.DocumentName + document.DocumentType);
    }
    [HttpGet]
    public async Task<IActionResult> ViewDocument(int id)
    {
      // Retrieve the document based on ApprovalProcessDetailDocID
      var document = await _appDBContext.HR_DocumentUploads
         .FirstOrDefaultAsync(d => d.DocumentUploadID == id);

      if (document == null || document.DocumentData == null)
      {
        // If document is not found, return a 404 error.
        return NotFound();
      }

      // Determine MIME type based on file extension
      string mimeType = document.DocumentType.ToLower() switch
      {
        ".pdf" => "application/pdf",
        ".jpg" => "image/jpeg",
        ".jpeg" => "image/jpeg",
        ".png" => "image/png",
        _ => "application/octet-stream"
      };

      // Return the document for inline viewing
      return File(document.DocumentData, mimeType, document.DocumentName+ document.DocumentType);
    }

  }
}

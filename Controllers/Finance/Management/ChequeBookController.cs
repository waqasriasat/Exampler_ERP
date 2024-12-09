using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace Exampler_ERP.Controllers.Finance.Management
{
  public class ChequeBookController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;

    public ChequeBookController(AppDBContext appDBContext, IConfiguration configuration, Utils utils)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
    }
    public async Task<IActionResult> Index(string searchBankName)
    {
      var ChequeBooksQuery = _appDBContext.FI_ChequeBooks
        .Include(b => b.BankAccount)
          .Where(b => b.DeleteYNID != 1);

      if (!string.IsNullOrEmpty(searchBankName))
      {
        ChequeBooksQuery = ChequeBooksQuery.Where(b => b.BankAccount.BankName.Contains(searchBankName));
      }

      var ChequeBooks = await ChequeBooksQuery.ToListAsync();

      if (!string.IsNullOrEmpty(searchBankName) && ChequeBooks.Count == 0)
      {
        TempData["ErrorMessage"] = "No ChequeBook found with the name '" + searchBankName + "'. Please check the name and try again.";
      }
      return View("~/Views/Finance/Management/ChequeBook/ChequeBook.cshtml", ChequeBooks);
    }


    public async Task<IActionResult> ChequeBook()
    {
      var ChequeBooks = await _appDBContext.FI_ChequeBooks.ToListAsync();
      return Ok(ChequeBooks);
    }// Add the Edit action
    public async Task<IActionResult> Edit(int id)
    {
      ViewBag.bankList = await _utils.Get_FI_BankList();
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
        var ChequeBook = await _appDBContext.FI_ChequeBooks
        .Where(b => b.ChequeBookID == id)
        .Include(b => b.ChequeDetails)
        .FirstOrDefaultAsync();

      if (ChequeBook == null)
      {
        return NotFound();
      }
      return PartialView("~/Views/Finance/Management/ChequeBook/EditChequeBook.cshtml", ChequeBook);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(FI_ChequeBook ChequeBook)
    {
      if (ModelState.IsValid)
      {
       
        _appDBContext.Update(ChequeBook);
        await _appDBContext.SaveChangesAsync();
        TempData["SuccessMessage"] = "ChequeBook updated successfully.";
        return Json(new { success = true });
      }
      return Json(new { success = false, message = "Error creating ChequeBook. Please check the inputs." });
    }
    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.bankList = await _utils.Get_FI_BankList();
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      return PartialView("~/Views/Finance/Management/ChequeBook/AddChequeBook.cshtml", new FI_ChequeBook());
    }

    [HttpPost]
    public async Task<IActionResult> Create(FI_ChequeBook ChequeBook)
    {
      if (ModelState.IsValid)
      {
        ChequeBook.TotalPages = (ChequeBook.ChequeTo - ChequeBook.ChequeFrom) + 1;

        ChequeBook.RegDate = DateTime.Now;
        ChequeBook.DeleteYNID = 0;

        _appDBContext.FI_ChequeBooks.Add(ChequeBook);
        await _appDBContext.SaveChangesAsync();

        var chequeDetails = new List<FI_ChequeBookDetail>();
        for (int i = ChequeBook.ChequeFrom; i <= ChequeBook.ChequeTo; i++)
        {
          chequeDetails.Add(new FI_ChequeBookDetail
          {
            ChequeBookID = ChequeBook.ChequeBookID,
            ChequeNo = i,
            GLID = 0,
            Status = "Unbind" // Default status
          });
        }

        _appDBContext.FI_ChequeBookDetails.AddRange(chequeDetails);
        await _appDBContext.SaveChangesAsync();

        TempData["SuccessMessage"] = "ChequeBook created successfully.";
        return Json(new { success = true });
      }

      return Json(new { success = false, message = "Error creating ChequeBook. Please check the inputs." });
    }

    public async Task<IActionResult> Delete(int id)
    {
      var ChequeBook = await _appDBContext.FI_ChequeBooks.FindAsync(id);
      if (ChequeBook == null)
      {
        return NotFound();
      }

      ChequeBook.ActiveYNID = 2;
      ChequeBook.DeleteYNID = 1;

      _appDBContext.FI_ChequeBooks.Update(ChequeBook);
      await _appDBContext.SaveChangesAsync();
      TempData["SuccessMessage"] = "ChequeBook deleted successfully.";

      return Json(new { success = true });
    }
    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var ChequeBooks = await _appDBContext.FI_ChequeBooks
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();

      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add("ChequeBooks");

        // Adding column headers
        worksheet.Cells["A1"].Value = "ChequeBook ID";
        worksheet.Cells["B1"].Value = "BankName";
        worksheet.Cells["C1"].Value = "ChequeFrom";
        worksheet.Cells["D1"].Value = "ChequeTo";
        worksheet.Cells["E1"].Value = "TotalPages";
   
        // Adding data rows
        for (int i = 0; i < ChequeBooks.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = ChequeBooks[i].ChequeBookID;
          worksheet.Cells[i + 2, 2].Value = ChequeBooks[i].BankAccount.BankName;
          worksheet.Cells[i + 2, 3].Value = ChequeBooks[i].ChequeFrom;
          worksheet.Cells[i + 2, 4].Value = ChequeBooks[i].ChequeTo;
          worksheet.Cells[i + 2, 5].Value = ChequeBooks[i].TotalPages;
        }

        // Apply formatting
        worksheet.Cells["A1:E1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();


        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = $"ChequeBooks-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
    public async Task<IActionResult> Print()
    {
      var ChequeBooks = await _appDBContext.FI_ChequeBooks
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();
      return View("~/Views/Finance/Management/ChequeBook/PrintChequeBook.cshtml", ChequeBooks);
    }


  }
}

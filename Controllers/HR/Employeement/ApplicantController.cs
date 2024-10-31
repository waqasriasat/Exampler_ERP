using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace Exampler_ERP.Controllers.HR.Employeement
{
  public class ApplicantController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    public ApplicantController(AppDBContext appDBContext, IConfiguration configuration, Utils utils)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
    }
    public async Task<IActionResult> Index(int? id)
    {
      var ApplicantsQuery = _appDBContext.HR_Applicants
          .Where(a => !_appDBContext.HR_Employees.Select(e => e.ApplicantID).Contains(a.ApplicantID));

      if (id.HasValue)
      {
        ApplicantsQuery = ApplicantsQuery.Where(a => a.ApplicantID == id.Value);
      }

      var Applicants = await ApplicantsQuery.ToListAsync();

      return View("~/Views/HR/Employeement/Applicant/Applicant.cshtml", Applicants);
    }

    public async Task<IActionResult> GetApplicantSuggestions(string term)
    {
      var EmployeeList = await _appDBContext.HR_Employees
          .Select(e => e.EmployeeID)
          .ToListAsync();
      var applicants = await _appDBContext.HR_Applicants
          .Where(b => !_appDBContext.HR_Employees.Select(e => e.ApplicantID).Contains(b.ApplicantID) &&
              (
                  // Search in Employee names
                  (b.FirstName + " " + b.FatherName + " " + b.FamilyName).Contains(term) ||
                  // Search in Department name
                  (b.BranchType.BranchTypeName != null && b.BranchType.BranchTypeName.Contains(term)) ||
                  // Search in Phone numbers
                  (b.Phone1 != null && b.Phone1.Contains(term)) ||
                  (b.Phone2 != null && b.Phone2.Contains(term)) ||
                  (b.Mobile != null && b.Mobile.Contains(term)) ||
                  (b.Whatsapp != null && b.Whatsapp.Contains(term)) ||
                  // Search in ID and Passport numbers
                  (b.IDNumber != null && b.IDNumber.Contains(term)) ||
                  (b.PassportNumber != null && b.PassportNumber.Contains(term))
              )
          )
          .Include(b => b.BranchType)
          .Select(e => new
          {
            // Label showing employee's full name
            label = e.FirstName + " " + e.FatherName + " " + e.FamilyName,
            id = e.ApplicantID,
            // Additional details like department, branch, and designation for display (optional)
            branch = e.BranchType.BranchTypeName,
            phone = e.Phone1 ?? e.Phone2 ?? e.Mobile, // Prefer Phone1, else Phone2, else Mobile
            whatsapp = e.Whatsapp,
            idNumber = e.IDNumber,
            passportNumber = e.PassportNumber
          })
          .ToListAsync();

      // Return the result as JSON for autocomplete
      return Json(applicants);
    }
    public async Task<IActionResult> Applicant()
    {
      var Applicants = await _appDBContext.HR_Applicants.ToListAsync();
      return Ok(Applicants);
    }// Add the Edit action
    public async Task<IActionResult> Edit(int id)
    {
      ViewBag.GenderList = await _utils.GetGender();
      ViewBag.MaritalStatusList = await _utils.GetMaritalStatus();
      ViewBag.ReligionList = await _utils.GetReligion();
      ViewBag.CountriesList = await _utils.GetCountries();
      ViewBag.BranchsList = await _utils.GetBranchs();
      ViewBag.DepartmentsList = await _utils.GetDepartments();
      ViewBag.DesignationsList = await _utils.GetDesignations();
      var Applicant = await _appDBContext.HR_Applicants.FindAsync(id);
      if (Applicant == null)
      {
        return NotFound();
      }
      return PartialView("~/Views/HR/Employeement/Applicant/EditApplicant.cshtml", Applicant);
    }
    [HttpPost]
    public async Task<IActionResult> Edit(HR_Applicant Applicant, IFormFile profilePicture, string ExistingPicture)
    {
      if (ModelState.IsValid)
      {
        if (profilePicture != null && profilePicture.Length > 0)
        {
          using (var memoryStream = new MemoryStream())
          {
            await profilePicture.CopyToAsync(memoryStream);
            Applicant.Picture = memoryStream.ToArray();
          }
        }
        else if (!string.IsNullOrEmpty(ExistingPicture))
        {
          Applicant.Picture = Convert.FromBase64String(ExistingPicture);
        }
        _appDBContext.Update(Applicant);
        await _appDBContext.SaveChangesAsync();
        TempData["SuccessMessage"] = "Applicant updated successfully.";
        return Json(new { success = true });
      }
      TempData["ErrorMessage"] = "Error updating Applicant. Please check the inputs.";
      return PartialView("~/Views/HR/Employeement/Applicant/EditApplicant.cshtml", Applicant);
    }
    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.GenderList = await _utils.GetGender();
      ViewBag.MaritalStatusList = await _utils.GetMaritalStatus();
      ViewBag.ReligionList = await _utils.GetReligion();
      ViewBag.CountriesList = await _utils.GetCountries();
      ViewBag.BranchsList = await _utils.GetBranchs();
      ViewBag.DepartmentsList = await _utils.GetDepartments();
      ViewBag.DesignationsList = await _utils.GetDesignations();
      return PartialView("~/Views/HR/Employeement/Applicant/AddApplicant.cshtml", new HR_Applicant());
    }
    [HttpPost]
    public async Task<IActionResult> Create(HR_Applicant Applicant, IFormFile profilePicture)
    {
      if (ModelState.IsValid)
      {
        if (profilePicture != null && profilePicture.Length > 0)
        {
          using (var memoryStream = new MemoryStream())
          {
            await profilePicture.CopyToAsync(memoryStream);
            Applicant.Picture = memoryStream.ToArray();
          }
        }

        _appDBContext.HR_Applicants.Add(Applicant);
        await _appDBContext.SaveChangesAsync();
        TempData["SuccessMessage"] = "Applicant created successfully.";
        return Json(new { success = true });
      }
      TempData["ErrorMessage"] = "Error creating Applicant. Please check the inputs.";

      return PartialView("~/Views/HR/Employeement/Applicant/AddApplicant.cshtml", Applicant);
    }
    public async Task<IActionResult> Delete(int id)
    {
      var Applicant = await _appDBContext.HR_Applicants.FindAsync(id);
      if (Applicant == null)
      {
        return NotFound();
      }

    
      _appDBContext.HR_Applicants.Update(Applicant);
      await _appDBContext.SaveChangesAsync();
      TempData["SuccessMessage"] = "Applicant deleted successfully.";
      return Json(new { success = true });
    }
    public async Task<IActionResult> Print()
    {
      var Applicants = await _appDBContext.HR_Applicants
          .Include(b => b.BranchType)
          .ToListAsync();

      ViewBag.GenderList = _utils.GetGender();
      ViewBag.MaritalStatusList = _utils.GetMaritalStatus();
      ViewBag.ReligionList = _utils.GetReligion();


      return View("~/Views/HR/Employeement/Applicant/PrintApplicant.cshtml", Applicants);
    }
    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var Applicants = await _appDBContext.HR_Applicants
          .Include(b => b.BranchType)
          .ToListAsync();

      var GenderList = await _utils.GetGender();
      var MaritalStatusList = await _utils.GetMaritalStatus();
      var ReligionList = await _utils.GetReligion();
      var CountriesList = await _utils.GetCountries();

      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add("Applicants");
        worksheet.Cells["B1"].Value = "Branch";
        worksheet.Cells["E1"].Value = "Applicant ID";
        worksheet.Cells["G1"].Value = "Applicant Name";
        worksheet.Cells["I1"].Value = "Gender";
        worksheet.Cells["J1"].Value = "DOB";
        worksheet.Cells["K1"].Value = "Marital Status";
        worksheet.Cells["L1"].Value = "No of Children";
        worksheet.Cells["M1"].Value = "Phone1";
        worksheet.Cells["N1"].Value = "Phone2";
        worksheet.Cells["O1"].Value = "Mobile";
        worksheet.Cells["P1"].Value = "WhatsApp";
        worksheet.Cells["Q1"].Value = "Religion";
        worksheet.Cells["R1"].Value = "Place of Birth";
        worksheet.Cells["S1"].Value = "Country";
        worksheet.Cells["T1"].Value = "Fax";
        worksheet.Cells["U1"].Value = "Email";
        worksheet.Cells["V1"].Value = "PO Box";
        worksheet.Cells["W1"].Value = "Address";
        worksheet.Cells["X1"].Value = "ID Number";
        worksheet.Cells["Y1"].Value = "ID Place of Issue";
        worksheet.Cells["AB1"].Value = "Passport Number";
        worksheet.Cells["AC1"].Value = "Passport Place of Issue";

        for (int i = 0; i < Applicants.Count; i++)
        {
          worksheet.Cells[i + 2, 2].Value = Applicants[i].BranchType?.BranchTypeName;
          worksheet.Cells[i + 2, 5].Value = Applicants[i].ApplicantID;
          worksheet.Cells[i + 2, 7].Value = Applicants[i].FirstName + ' ' + Applicants[i].FatherName + ' ' + Applicants[i].FamilyName;
          worksheet.Cells[i + 2, 9].Value = Applicants[i].Sex == 0 || Applicants[i].Sex == null
              ? ""
              : GenderList.FirstOrDefault(g => g.Value == Applicants[i].Sex.ToString())?.Text;
          worksheet.Cells[i + 2, 10].Value = Applicants[i].DOB?.ToString("dd-MMM-yyyy");
          worksheet.Cells[i + 2, 11].Value = Applicants[i].MaritalStatus == 0 || Applicants[i].MaritalStatus == null
              ? ""
              : MaritalStatusList.FirstOrDefault(m => m.Value == Applicants[i].MaritalStatus.ToString())?.Text;
          worksheet.Cells[i + 2, 12].Value = Applicants[i].NoOfChildren;
          worksheet.Cells[i + 2, 13].Value = Applicants[i].Phone1;
          worksheet.Cells[i + 2, 14].Value = Applicants[i].Phone2;
          worksheet.Cells[i + 2, 15].Value = Applicants[i].Mobile;
          worksheet.Cells[i + 2, 16].Value = Applicants[i].Whatsapp;
          worksheet.Cells[i + 2, 17].Value = Applicants[i].Religion == 0 || Applicants[i].Religion == null
              ? ""
              : ReligionList.FirstOrDefault(r => r.Value == Applicants[i].Religion.ToString())?.Text;
          worksheet.Cells[i + 2, 18].Value = Applicants[i].PlaceOfBirth;
          worksheet.Cells[i + 2, 19].Value = Applicants[i].CountryID == 0 || Applicants[i].CountryID == null
              ? ""
              : CountriesList.FirstOrDefault(c => c.Value == Applicants[i].CountryID.ToString())?.Text;
          worksheet.Cells[i + 2, 20].Value = Applicants[i].Fax;
          worksheet.Cells[i + 2, 21].Value = Applicants[i].Email;
          worksheet.Cells[i + 2, 22].Value = Applicants[i].POBox;
          worksheet.Cells[i + 2, 23].Value = Applicants[i].Address;
          worksheet.Cells[i + 2, 24].Value = Applicants[i].IDNumber;
          worksheet.Cells[i + 2, 25].Value = Applicants[i].IDPlaceOfIssue;
          worksheet.Cells[i + 2, 28].Value = Applicants[i].PassportNumber;
          worksheet.Cells[i + 2, 29].Value = Applicants[i].PassportPlaceOfIssue;
        }

        worksheet.Cells["G1"].Style.Font.Bold = true;
        worksheet.Cells["A1"].Style.Numberformat.Format = "dd-mmm-yyyy";
        worksheet.Cells["J1"].Style.Numberformat.Format = "dd-mmm-yyyy";
        worksheet.Cells["Z1"].Style.Numberformat.Format = "dd-mmm-yyyy";
        worksheet.Cells["AA1"].Style.Numberformat.Format = "dd-mmm-yyyy";
        worksheet.Cells["AD1"].Style.Numberformat.Format = "dd-mmm-yyyy";
        worksheet.Cells["AE1"].Style.Numberformat.Format = "dd-mmm-yyyy";
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = $"Applicants-{DateTime.Now:yyyyMMddHHmmssfff}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
  }
}

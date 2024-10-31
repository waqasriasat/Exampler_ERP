using Exampler_ERP.Models;
using Exampler_ERP.Models.Temp;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace Exampler_ERP.Controllers.HR.Employeement
{
  public class EmployeeController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;


    public EmployeeController(AppDBContext appDBContext, IConfiguration configuration, Utils utils)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
    }
    public async Task<IActionResult> Index(int? id)
    {
      var employeesQuery = _appDBContext.HR_Employees
          .Where(e => e.DeleteYNID != 1);  // Ensure employees are not marked as deleted

      if (id.HasValue)
      {
        employeesQuery = employeesQuery.Where(e => e.EmployeeID == id.Value);
      }

      var employees = await employeesQuery.ToListAsync();

      return View("~/Views/HR/Employeement/Employee/Employee.cshtml", employees);
    }


    public async Task<IActionResult> GetEmployeeSuggestions(string term)
    {
      return Json(await _utils.GetSearchingEmployee(term));
    }

    public async Task<IActionResult> Employee()
    {
      var employees = await _appDBContext.HR_Employees.ToListAsync();
      return Ok(employees);
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
      var employee = await _appDBContext.HR_Employees.FindAsync(id);
      if (employee == null)
      {
        return NotFound();
      }
      employee.UserName = CR_CipherKey.Decrypt(employee.UserName);
      employee.Password = CR_CipherKey.Decrypt(employee.Password);
      return PartialView("~/Views/HR/Employeement/Employee/EditEmployee.cshtml", employee);
    }
    [HttpPost]
    public async Task<IActionResult> Edit(HR_Employee employee, IFormFile profilePicture, string ExistingPicture)
    {
      if (ModelState.IsValid)
      {
        if (profilePicture != null && profilePicture.Length > 0)
        {
          using (var memoryStream = new MemoryStream())
          {
            await profilePicture.CopyToAsync(memoryStream);
            employee.Picture = memoryStream.ToArray();
          }
        }
        else if (!string.IsNullOrEmpty(ExistingPicture))
        {
          employee.Picture = Convert.FromBase64String(ExistingPicture);
        }
        employee.UserName = CR_CipherKey.Encrypt(employee.UserName);
        employee.Password = CR_CipherKey.Encrypt(employee.Password);
        _appDBContext.Update(employee);
        await _appDBContext.SaveChangesAsync();
        TempData["SuccessMessage"] = "Employee Updated successfully.";
        return Json(new { success = true });
      }
      TempData["ErrorMessage"] = "Error Updating Role. Please check the inputs.";
      return PartialView("~/Views/HR/Employeement/Employee/EditEmployee.cshtml", employee);
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
      return PartialView("~/Views/HR/Employeement/Employee/AddEmployee.cshtml", new HR_Employee());
    }
    [HttpPost]
    public async Task<IActionResult> Create(HR_Employee employee, IFormFile profilePicture)
    {
      if (ModelState.IsValid)
      {
        if (profilePicture != null && profilePicture.Length > 0)
        {
          using (var memoryStream = new MemoryStream())
          {
            await profilePicture.CopyToAsync(memoryStream);
            employee.Picture = memoryStream.ToArray();
          }
        }
        employee.UserName = CR_CipherKey.Encrypt(employee.UserName);
        employee.Password = CR_CipherKey.Encrypt(employee.Password);
        employee.DeleteYNID = 0;
        employee.ActiveYNID = 2;
        _appDBContext.HR_Employees.Add(employee);
        await _appDBContext.SaveChangesAsync();

        var employeeId = employee.EmployeeID;
        if (employeeId > 0)
        {
          var processCount = await _appDBContext.CR_ProcessTypeApprovalSetups
                              .Where(pta => pta.ProcessTypeID > 0 && pta.ProcessTypeID == 2)
                              .CountAsync();

          if (processCount > 0)
          {
            var newProcessTypeApproval = new CR_ProcessTypeApproval
            {
              ProcessTypeID = 2,
              Notes = "Create New Employee",
              Date = DateTime.Now,
              EmployeeID = employee.EmployeeID,
              UserID = HttpContext.Session.GetInt32("UserID") ?? default(int),
              TransactionID = employee.EmployeeID
            };

            _appDBContext.CR_ProcessTypeApprovals.Add(newProcessTypeApproval);
            await _appDBContext.SaveChangesAsync();

            var nextApprovalSetup = await _appDBContext.CR_ProcessTypeApprovalSetups
                                        .Where(pta => pta.ProcessTypeID == 2 && pta.Rank == 1)
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
            employee.ActiveYNID = 1;
            _appDBContext.HR_Employees.Update(employee);
            await _appDBContext.SaveChangesAsync();
            TempData["SuccessMessage"] = "Employee Created successfully. No process setup found, Employee activated.";
            return Json(new { success = true, message = "No process setup found, Employee activated." });
          }
        }
        TempData["SuccessMessage"] = "Employee Created successfully. Continue to the Approval Process Setup for Employee Activation.";
        return Json(new { success = true });
      }
      TempData["ErrorMessage"] = "Error creating Employee. Please check the inputs.";
      return PartialView("~/Views/HR/Employeement/Employee/AddEmployee.cshtml", employee);
    }
    public async Task<IActionResult> Delete(int id)
    {
      var employee = await _appDBContext.HR_Employees.FindAsync(id);
      if (employee == null)
      {
        return NotFound();
      }

      employee.ActiveYNID = 2;
      employee.DeleteYNID = 1;

      _appDBContext.HR_Employees.Update(employee);
      await _appDBContext.SaveChangesAsync();
      TempData["SuccessMessage"] = "Employee Deleted successfully.";
      return Json(new { success = true });
    }
    public async Task<IActionResult> Print()
    {
      var employees = await _appDBContext.HR_Employees
          .Where(e => e.DeleteYNID != 1)
          .Include(e => e.BranchType)
         .Include(e => e.DepartmentType)
         .Include(e => e.DesignationType)
          .ToListAsync();

      ViewBag.GenderList = _utils.GetGender();
      ViewBag.MaritalStatusList = _utils.GetMaritalStatus();
      ViewBag.ReligionList = _utils.GetReligion();



      return View("~/Views/HR/Employeement/Employee/PrintEmployee.cshtml", employees);
    }
    public async Task<IActionResult> PrintEmployeeBioData(int id)
    {
      var employee = await _appDBContext.HR_Employees
        .Where(e => e.DeleteYNID != 1 && e.EmployeeID == id)
        .FirstOrDefaultAsync();

      if (employee == null)
      {
        return NotFound(); // Handle employee not found
      }

      var educations = await _appDBContext.HR_EmployeeEducations
          .Where(edu => edu.EmployeeID == id)
          .ToListAsync();

      var experiences = await _appDBContext.HR_EmployeeExperiences
          .Where(exp => exp.EmployeeID == id)
          .ToListAsync();

      var employeeBioData = new EmployeeBioDataViewModel
      {
        Employee = employee,
        Educations = educations,
        Experiences = experiences
      };

      ViewBag.GenderList = await _utils.GetGender();
      ViewBag.MaritalStatusList = await _utils.GetMaritalStatus();
      ViewBag.ReligionList = await _utils.GetReligion();
      ViewBag.QualificationTypes = await _utils.GetQualifications();
      ViewBag.CountryTypes = await _utils.GetCountries();

      return View("~/Views/HR/Employeement/Employee/PrintEmployeeBioData.cshtml", employeeBioData);
    }
    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var employees = await _appDBContext.HR_Employees
          .Where(e => e.DeleteYNID != 1)
          .Include(e => e.BranchType)
         .Include(e => e.DepartmentType)
         .Include(e => e.DesignationType)
          .ToListAsync();
      var GenderList = await _utils.GetGender();
      var MaritalStatusList = await _utils.GetMaritalStatus();
      var ReligionList = await _utils.GetReligion();
      var CountriesList = await _utils.GetCountries();
      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add("Employees");
        worksheet.Cells["A1"].Value = "Hire Date";
        worksheet.Cells["B1"].Value = "Branch";
        worksheet.Cells["C1"].Value = "Department";
        worksheet.Cells["D1"].Value = "Designation";
        worksheet.Cells["E1"].Value = "Employee ID";
        worksheet.Cells["F1"].Value = "Employee Code";
        worksheet.Cells["G1"].Value = "Employee Name";
        worksheet.Cells["H1"].Value = "Active";
        worksheet.Cells["I1"].Value = "Gender";
        worksheet.Cells["J1"].Value = "DOB";
        worksheet.Cells["K1"].Value = "Marital Status";
        worksheet.Cells["L1"].Value = "No of Children";
        worksheet.Cells["M1"].Value = "Phone1";
        worksheet.Cells["N1"].Value = "Phone2";
        worksheet.Cells["O1"].Value = "Mobile";
        worksheet.Cells["P1"].Value = "WhatsApp";
        worksheet.Cells["Q1"].Value = "Religen";
        worksheet.Cells["R1"].Value = "Place of Birth";
        worksheet.Cells["S1"].Value = "Country";
        worksheet.Cells["T1"].Value = "Fax";
        worksheet.Cells["U1"].Value = "Email";
        worksheet.Cells["V1"].Value = "PO Box";
        worksheet.Cells["W1"].Value = "Address";
        worksheet.Cells["X1"].Value = "ID Number";
        worksheet.Cells["Y1"].Value = "ID Place of Issue";
        worksheet.Cells["Z1"].Value = "ID Issue Date";
        worksheet.Cells["AA1"].Value = "ID Expiry Date";
        worksheet.Cells["AB1"].Value = "Passport Number";
        worksheet.Cells["AC1"].Value = "Passport Place of Issue";
        worksheet.Cells["AD1"].Value = "Passport Issue Date";
        worksheet.Cells["AE1"].Value = "Passport Expiry Date";
        for (int i = 0; i < employees.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = employees[i].HireDate?.ToString("dd-MMM-yyyy");
          worksheet.Cells[i + 2, 2].Value = employees[i].BranchType?.BranchTypeName;
          worksheet.Cells[i + 2, 3].Value = employees[i].DepartmentType?.DepartmentTypeName;
          worksheet.Cells[i + 2, 4].Value = employees[i].DesignationType?.DesignationTypeName;
          worksheet.Cells[i + 2, 5].Value = employees[i].EmployeeID;
          worksheet.Cells[i + 2, 6].Value = employees[i].EmployeeCode;
          worksheet.Cells[i + 2, 7].Value = employees[i].FirstName + ' ' + employees[i].FatherName + ' ' + employees[i].FamilyName;
          worksheet.Cells[i + 2, 8].Value = employees[i].ActiveYNID == 1 ? "Yes" : "No";
          worksheet.Cells[i + 2, 9].Value = employees[i].Sex == 0 || employees[i].Sex == null
          ? ""
          : GenderList.FirstOrDefault(g => g.Value == employees[i].Sex.ToString())?.Text;
          worksheet.Cells[i + 2, 10].Value = employees[i].DOB?.ToString("dd-MMM-yyyy");
          worksheet.Cells[i + 2, 11].Value = employees[i].MaritalStatus == 0 || employees[i].MaritalStatus == null
          ? ""
          : MaritalStatusList.FirstOrDefault(m => m.Value == employees[i].MaritalStatus.ToString())?.Text;
          worksheet.Cells[i + 2, 12].Value = employees[i].NoOfChildren;
          worksheet.Cells[i + 2, 13].Value = employees[i].Phone1;
          worksheet.Cells[i + 2, 14].Value = employees[i].Phone2;
          worksheet.Cells[i + 2, 15].Value = employees[i].Mobile;
          worksheet.Cells[i + 2, 16].Value = employees[i].Whatsapp;
          worksheet.Cells[i + 2, 17].Value = employees[i].Religion == 0 || employees[i].Religion == null
          ? ""
          : ReligionList.FirstOrDefault(r => r.Value == employees[i].Religion.ToString())?.Text;
          worksheet.Cells[i + 2, 18].Value = employees[i].PlaceOfBirth;
          worksheet.Cells[i + 2, 19].Value = employees[i].CountryID == 0 || employees[i].CountryID == null
          ? ""
          : CountriesList.FirstOrDefault(c => c.Value == employees[i].CountryID.ToString())?.Text;
          worksheet.Cells[i + 2, 20].Value = employees[i].Fax;
          worksheet.Cells[i + 2, 21].Value = employees[i].Email;
          worksheet.Cells[i + 2, 22].Value = employees[i].POBox;
          worksheet.Cells[i + 2, 23].Value = employees[i].Address;
          worksheet.Cells[i + 2, 24].Value = employees[i].IDNumber;
          worksheet.Cells[i + 2, 25].Value = employees[i].IDPlaceOfIssue;
          worksheet.Cells[i + 2, 26].Value = employees[i].IDIssueDate?.ToString("dd-MMM-yyyy");
          worksheet.Cells[i + 2, 27].Value = employees[i].IDExpiryDate?.ToString("dd-MMM-yyyy");
          worksheet.Cells[i + 2, 28].Value = employees[i].PassportNumber;
          worksheet.Cells[i + 2, 29].Value = employees[i].PassportPlaceOfIssue;
          worksheet.Cells[i + 2, 30].Value = employees[i].PassportIssueDate?.ToString("dd-MMM-yyyy");
          worksheet.Cells[i + 2, 31].Value = employees[i].PassportExpiryDate?.ToString("dd-MMM-yyyy");

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
        string excelName = $"Employees-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
    public IActionResult GetEmployeePicture(int userId)
    {
      var employeePicture = (from emp in _appDBContext.HR_Employees
                             join usr in _appDBContext.CR_Users on emp.EmployeeID equals usr.EmployeeID
                             where usr.UserID == userId
                             select emp.Picture).FirstOrDefault();

      if (employeePicture != null)
      {
        return File(employeePicture, "image/jpeg"); // or "image/png" depending on your image format
      }
      else
      {
        var noImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/icons/NoImage.jpg");
        var noImageFile = System.IO.File.ReadAllBytes(noImagePath);
        return File(noImageFile, "image/jpeg");
      }
    }
    [HttpGet]
    public async Task<IActionResult> ChangePassword()
    {
      var employeeID = HttpContext.Session.GetInt32("EmployeeID"); // Assume EmployeeID is stored in session
      if (employeeID == null)
      {
        return RedirectToAction("Login", "Auth"); // Redirect to login if not logged in
      }

      var employee = await _appDBContext.HR_Employees.FindAsync(employeeID);
      if (employee == null)
      {
        return NotFound();
      }

      var model = new ChangePasswordModel
      {
        EmployeeID = employee.EmployeeID,
        UserName = CR_CipherKey.Decrypt(employee.UserName)
      };

      return PartialView("~/Views/HR/Employeement/Employee/ChangePassword.cshtml", model);
    }
    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
    {
      if (ModelState.IsValid)
      {
        var employee = await _appDBContext.HR_Employees.FindAsync(model.EmployeeID);
        if (employee == null)
        {
          return NotFound();
        }

        // Verify old password
        if (employee.Password != CR_CipherKey.Encrypt(model.OldPassword)) // Assuming passwords are stored in plain text (you should hash them)
        {
          ModelState.AddModelError("", "Old password is incorrect.");
          var modelChangePassword = new ChangePasswordModel
          {
            EmployeeID = employee.EmployeeID,
            UserName = CR_CipherKey.Decrypt(employee.UserName)
          };
          return PartialView("~/Views/HR/Employeement/Employee/ChangePassword.cshtml", modelChangePassword);
        }

        // Update with new password
        employee.Password = CR_CipherKey.Encrypt(model.NewPassword);

        // Save changes
        await _appDBContext.SaveChangesAsync();

        TempData["SuccessMessage"] = "Password changed successfully!";

        // Return a JSON response for success
        return Json(new { success = true, message = "Password changed successfully!" });
      }


      return PartialView("~/Views/HR/Employeement/Employee/ChangePassword.cshtml", model);
    }
    [HttpGet]
    public async Task<IActionResult> ChangePicture()
    {
      var employeeID = HttpContext.Session.GetInt32("EmployeeID");
      if (employeeID == null)
      {
        return RedirectToAction("Login", "Auth"); // Redirect to login if not logged in
      }

      var employee = await _appDBContext.HR_Employees.FindAsync(employeeID);
      if (employee == null)
      {
        return NotFound();
      }

      var model = new ChangePictureModel
      {
        EmployeeID = employee.EmployeeID,
        Picture = employee.Picture
      };

      return PartialView("~/Views/HR/Employeement/Employee/ChangePicture.cshtml", model);
    }
    [HttpPost]
    public async Task<IActionResult> ChangePicture(ChangePictureModel model, IFormFile profilePicture, string ExistingPicture)
    {
      if (profilePicture != null && profilePicture.Length > 0)
      {
        using (var memoryStream = new MemoryStream())
        {
          await profilePicture.CopyToAsync(memoryStream);
          model.Picture = memoryStream.ToArray();
        }
      }
      else if (!string.IsNullOrEmpty(ExistingPicture))
      {
        model.Picture = Convert.FromBase64String(ExistingPicture);
      }
      var employee = await _appDBContext.HR_Employees.FindAsync(model.EmployeeID);
      if (employee == null)
      {
        return NotFound();
      }

      // Update the employee's picture
      employee.Picture = model.Picture;
      await _appDBContext.SaveChangesAsync();

      TempData["SuccessMessage"] = "Profile picture updated successfully!";
      return Json(new { success = true, message = "Profile picture updated successfully!" });

    }
    public async Task<IActionResult> DirectManager(int id)
    {
      var employee = await _appDBContext.HR_Employees.FindAsync(id);

      if (employee == null)
      {
        return NotFound();
      }

      var employees = await _appDBContext.HR_Employees
          .Where(e => e.EmployeeID != id) // Ensure employee can't report to themselves
          .Select(e => new SelectListItem
          {
            Value = e.EmployeeID.ToString(),
            Text = e.FirstName + " " + e.FamilyName
          }).ToListAsync();

      var viewModel = new UpdateReportToViewModel
      {
        EmployeeID = employee.EmployeeID,
        ReportToID = employee.ReportToID
      };
      ViewBag.DirectManagerList = await _utils.GetDirectManager();
      return PartialView("~/Views/HR/Employeement/Employee/DirectManagerEmployee.cshtml", viewModel);
    }
    [HttpPost]
    public async Task<IActionResult> DirectManager(UpdateReportToViewModel model)
    {
      var employee = await _appDBContext.HR_Employees.FindAsync(model.EmployeeID);

      if (employee == null)
      {
        return NotFound();
      }

      if (model.ReportToID == model.EmployeeID)
      {
        ModelState.AddModelError("", "An employee cannot report to themselves.");
        return View(model); // Return with validation error
      }

      employee.ReportToID = model.ReportToID;

      await _appDBContext.SaveChangesAsync();

      return Json(new { success = true, message = "Direct Manager updated successfully!" });
    }
    public async Task<IActionResult> Education(int id)
    {
      // Create a new education form (empty form)
      var education = new HR_EmployeeEducation();
      ViewBag.EmployeeID = id.ToString();

      // Populate ViewBags for dropdowns like QualificationType, CountryType, etc.
      ViewBag.QualificationTypes = await _utils.GetQualifications();
      ViewBag.CountryTypes = await _utils.GetCountries();
      ViewBag.MonthTypes = await _utils.GetMonthsTypes();

      // Fetch the existing education records for the employee
      var employeeEducations = await _appDBContext.HR_EmployeeEducations
          .Where(e => e.EmployeeID == id)
          .ToListAsync();

      // Return the main view with the empty form and list of records
      return PartialView("~/Views/HR/Employeement/Employee/EmployeeEducation.cshtml", new EmployeeEducationViewModel
      {
        NewEducation = education,
        EmployeeEducations = employeeEducations
      });
    }
    [HttpPost]
    public async Task<IActionResult> Education(EmployeeEducationViewModel model, IFormFile? pdfFile)
    {
      if (ModelState.IsValid)
      {
        if (pdfFile != null && pdfFile.Length > 0)
        {
          // Ensure the file is a PDF
          if (pdfFile.ContentType == "application/pdf" && Path.GetExtension(pdfFile.FileName).ToLower() == ".pdf")
          {
            using (var memoryStream = new MemoryStream())
            {
              await pdfFile.CopyToAsync(memoryStream);
              model.NewEducation.DocImage = memoryStream.ToArray();  // Store PDF as binary
            }

            model.NewEducation.DocExt = Path.GetExtension(pdfFile.FileName).ToLower();
          }
          else
          {
            ModelState.AddModelError("", "Only PDF files are allowed.");
            return PartialView("~/Views/HR/Employeement/Employee/EmployeeEducation.cshtml", model);
          }
        }

        if (model.NewEducation.EducationID == 0)
        {
          _appDBContext.HR_EmployeeEducations.Add(model.NewEducation);
        }
        else
        {
          _appDBContext.HR_EmployeeEducations.Update(model.NewEducation);
        }

        await _appDBContext.SaveChangesAsync();

        return Json(new { success = true });
      }
      model.EmployeeEducations = await _appDBContext.HR_EmployeeEducations
        .Where(e => e.EmployeeID == model.NewEducation.EmployeeID)
        .ToListAsync();

      // Populate ViewBags for dropdowns
      ViewBag.QualificationTypes = await _utils.GetQualifications();
      ViewBag.CountryTypes = await _utils.GetCountries();
      ViewBag.MonthTypes = await _utils.GetMonthsTypes();

      return PartialView("~/Views/HR/Employeement/Employee/EmployeeEducation.cshtml", model);
    }
    [HttpGet]
    public async Task<IActionResult> EducationDocumentDownload(int id)
    {
      var document = await _appDBContext.HR_EmployeeEducations
        .Include(d => d.QualificationType)
          .FirstOrDefaultAsync(d => d.EducationID == id);

      if (document == null || document.DocImage == null)
      {
        return NotFound();
      }

      return File(document.DocImage, "application/octet-stream", "(" + document.EmployeeID + ") " + document.QualificationType?.QualificationTypeName + document.DocExt);
    }
    [HttpGet]
    public async Task<IActionResult> EducationDocumentView(int id)
    {
      var document = await _appDBContext.HR_EmployeeEducations
        .Include(d => d.QualificationType)
          .FirstOrDefaultAsync(d => d.EducationID == id);

      if (document == null || document.DocImage == null)
      {
        // If document is not found, return a 404 error.
        return NotFound();
      }

      // Determine MIME type based on file extension
      string mimeType = document.DocExt.ToLower() switch
      {
        ".pdf" => "application/pdf",   // Ensure PDF is returned correctly
        ".jpg" => "image/jpeg",
        ".jpeg" => "image/jpeg",
        ".png" => "image/png",
        _ => "application/octet-stream"
      };

      // Return the document for inline viewing
      return File(document.DocImage, mimeType, "("+document.EmployeeID+") "+document.QualificationType?.QualificationTypeName+document.DocExt);
    }
    public async Task<IActionResult> DeleteEmployeeEducation(int id)
    {
      var education = await _appDBContext.HR_EmployeeEducations.FindAsync(id);
      if (education == null)
      {
        return NotFound();
      }

      _appDBContext.HR_EmployeeEducations.Remove(education);
      await _appDBContext.SaveChangesAsync();

      return Json(new { success = true });
    }

    //////
    ///

    public async Task<IActionResult> Experience(int id)
    {
      // Create a new Experience form (empty form)
      var Experience = new HR_EmployeeExperience();
      ViewBag.EmployeeID = id.ToString();

      // Populate ViewBags for dropdowns like QualificationType, CountryType, etc.
      ViewBag.QualificationTypes = await _utils.GetQualifications();
      ViewBag.CountryTypes = await _utils.GetCountries();
      ViewBag.MonthTypes = await _utils.GetMonthsTypes();

      // Fetch the existing Experience records for the employee
      var employeeExperiences = await _appDBContext.HR_EmployeeExperiences
          .Where(e => e.EmployeeID == id)
          .ToListAsync();

      // Return the main view with the empty form and list of records
      return PartialView("~/Views/HR/Employeement/Employee/EmployeeExperience.cshtml", new EmployeeExperienceViewModel
      {
        NewExperience = Experience,
        EmployeeExperiences = employeeExperiences
      });
    }
    [HttpPost]
    public async Task<IActionResult> Experience(EmployeeExperienceViewModel model, IFormFile? pdfFile)
    {
      if (ModelState.IsValid)
      {
        if (pdfFile != null && pdfFile.Length > 0)
        {
          // Ensure the file is a PDF
          if (pdfFile.ContentType == "application/pdf" && Path.GetExtension(pdfFile.FileName).ToLower() == ".pdf")
          {
            using (var memoryStream = new MemoryStream())
            {
              await pdfFile.CopyToAsync(memoryStream);
              model.NewExperience.DocImage = memoryStream.ToArray();  // Store PDF as binary
            }

            model.NewExperience.DocExt = Path.GetExtension(pdfFile.FileName).ToLower();
          }
          else
          {
            ModelState.AddModelError("", "Only PDF files are allowed.");
            return PartialView("~/Views/HR/Employeement/Employee/EmployeeExperience.cshtml", model);
          }
        }

        if (model.NewExperience.ExperienceID == 0)
        {
          _appDBContext.HR_EmployeeExperiences.Add(model.NewExperience);
        }
        else
        {
          _appDBContext.HR_EmployeeExperiences.Update(model.NewExperience);
        }

        await _appDBContext.SaveChangesAsync();

        return Json(new { success = true });
      }
      model.EmployeeExperiences = await _appDBContext.HR_EmployeeExperiences
        .Where(e => e.EmployeeID == model.NewExperience.EmployeeID)
        .ToListAsync();

      // Populate ViewBags for dropdowns
      ViewBag.QualificationTypes = await _utils.GetQualifications();
      ViewBag.CountryTypes = await _utils.GetCountries();
      ViewBag.MonthTypes = await _utils.GetMonthsTypes();

      return PartialView("~/Views/HR/Employeement/Employee/EmployeeExperience.cshtml", model);
    }
    [HttpGet]
    public async Task<IActionResult> ExperienceDocumentDownload(int id)
    {
      var document = await _appDBContext.HR_EmployeeExperiences
          .FirstOrDefaultAsync(d => d.ExperienceID == id);

      if (document == null || document.DocImage == null)
      {
        return NotFound();
      }

      return File(document.DocImage, "application/octet-stream", "(" + document.EmployeeID + ") " + document.CompanyName + document.DocExt);
    }
    [HttpGet]
    public async Task<IActionResult> ExperienceDocumentView(int id)
    {
      var document = await _appDBContext.HR_EmployeeExperiences
          .FirstOrDefaultAsync(d => d.ExperienceID == id);

      if (document == null || document.DocImage == null)
      {
        // If document is not found, return a 404 error.
        return NotFound();
      }

      // Determine MIME type based on file extension
      string mimeType = document.DocExt.ToLower() switch
      {
        ".pdf" => "application/pdf",   // Ensure PDF is returned correctly
        ".jpg" => "image/jpeg",
        ".jpeg" => "image/jpeg",
        ".png" => "image/png",
        _ => "application/octet-stream"
      };

      // Return the document for inline viewing
      return File(document.DocImage, mimeType, "(" + document.EmployeeID + ") " + document.CompanyName + document.DocExt);
    }
    public async Task<IActionResult> DeleteEmployeeExperience(int id)
    {
      var Experience = await _appDBContext.HR_EmployeeExperiences.FindAsync(id);
      if (Experience == null)
      {
        return NotFound();
      }

      _appDBContext.HR_EmployeeExperiences.Remove(Experience);
      await _appDBContext.SaveChangesAsync();

      return Json(new { success = true });
    }
  }
}

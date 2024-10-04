using Exampler_ERP.Models;
using Exampler_ERP.Models.Temp;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
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
    public async Task<IActionResult> Index()
    {
      var employees = await _appDBContext.HR_Employees
                .Where(e => e.DeleteYNID != 1)
                .ToListAsync();

      return View("~/Views/HR/Employeement/Employee/Employee.cshtml", employees);
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
      ViewBag.CountriesList = _utils.GetCountries();
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
        return Json(new { success = true });
      }
      return PartialView("~/Views/HR/Employeement/Employee/EditEmployee.cshtml", employee);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.GenderList = await _utils.GetGender();
      ViewBag.MaritalStatusList = await _utils.GetMaritalStatus();
      ViewBag.ReligionList = await _utils.GetReligion();
      ViewBag.CountriesList = _utils.GetCountries();
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
        employee.ActiveYNID = 0;
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
                ApprovalProcessID = newProcessTypeApproval.ApprovalProcessID,
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
            return Json(new { success = true, message = "No process setup found, User activated." });
          }
        }

        return Json(new { success = true });
      }

      return PartialView("~/Views/HR/Employeement/Employee/AddEmployee.cshtml", employee);
    }
    public async Task<IActionResult> Delete(int id)
    {
      var employee = await _appDBContext.HR_Employees.FindAsync(id);
      if (employee == null)
      {
        return NotFound();
      }

      employee.ActiveYNID = 0;
      employee.DeleteYNID = 1;

      _appDBContext.HR_Employees.Update(employee);
      await _appDBContext.SaveChangesAsync();

      return Json(new { success = true });
    }
    public async Task<IActionResult> Print()
    {
      var employees = await _appDBContext.HR_Employees
          .Where(b => b.DeleteYNID != 1)
          .Include(b => b.BranchType)
         .Include(b => b.DepartmentType)
         .Include(b => b.DesignationType)
          .ToListAsync();

      ViewBag.GenderList = _utils.GetGender();
      ViewBag.MaritalStatusList = _utils.GetMaritalStatus();
      ViewBag.ReligionList = _utils.GetReligion();


      return View("~/Views/HR/Employeement/Employee/PrintEmployee.cshtml", employees);
    }
    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var employees = await _appDBContext.HR_Employees
          .Where(b => b.DeleteYNID != 1)
          .Include(b => b.BranchType)
         .Include(b => b.DepartmentType)
         .Include(b => b.DesignationType)
          .ToListAsync();
      var GenderList = await _utils.GetGender();
      var MaritalStatusList = await _utils.GetMaritalStatus();
      var ReligionList = await _utils.GetReligion();
      var CountriesList = _utils.GetCountries();
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
          worksheet.Cells[i + 2, 7].Value = employees[i].FirstName +' '+ employees[i].FatherName + ' ' + employees[i].FamilyName;
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

  }
}

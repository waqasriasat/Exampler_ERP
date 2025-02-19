using Exampler_ERP.Models;
using Exampler_ERP.Models.Temp;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace Exampler_ERP.Controllers.StoreManagement.StoreManagement
{
  public class MaterialRequisitionController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _conSTguration;
    private readonly Utils _utils;

    public MaterialRequisitionController(AppDBContext appDBContext, IConfiguration conSTguration, Utils utils)
    {
      _appDBContext = appDBContext;
      _conSTguration = conSTguration;
      _utils = utils;
    }
    public async Task<IActionResult> Index(string searchItemName)
    {
      if (HttpContext.Session.GetInt32("UserID") != null)
      {
        int UserID = int.Parse(HttpContext.Session.GetInt32("UserID").ToString());
        int departmentID = await _utils.PostUserIDGetDepartmentID(UserID);
        var MaterialRequisitionsQuery = _appDBContext.ST_MaterialRequisitions
          .Include(v => v.MaterialRequisitionDetails)
          .Include(v => v.RequisitionStatusTypes)
          .Where(v => v.DepartmentTypeID == departmentID);

        if (!string.IsNullOrEmpty(searchItemName))
        {
          MaterialRequisitionsQuery = MaterialRequisitionsQuery
              .Where(v => v.MaterialRequisitionDetails
                  .Any(d => d.Items.ItemName
                      .Contains(searchItemName)));
        }

        var MaterialRequisitions = await MaterialRequisitionsQuery.ToListAsync();

        if (!string.IsNullOrEmpty(searchItemName) && MaterialRequisitions.Count == 0)
        {
          TempData["ErrorMessage"] = "No Material Requisition found with the name '" + searchItemName + "'. Please check the name and try again.";
        }

        return View("~/Views/StoreManagement/StoreManagement/MaterialRequisition/MaterialRequisition.cshtml", MaterialRequisitions);
      }
      else
      {
        return RedirectToAction("Login", "Auth");
      }
    }
    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.ItemList = await _utils.GetItemList();
      ViewBag.ItemNameList = await _utils.GetItemList();

      ST_MaterialRequisition Requisitions = new ST_MaterialRequisition();
      Requisitions.MaterialRequisitionDetails.Add(new ST_MaterialRequisitionDetail() { RequisitionID = 0 });
      var model = new MaterialRequisitionsIndexViewModel
      {
        MaterialRequisitions = Requisitions
      };

      return PartialView("~/Views/StoreManagement/StoreManagement/MaterialRequisition/AddMaterialRequisition.cshtml", model);
    }


    [HttpPost]
    public async Task<IActionResult> Create(MaterialRequisitionsIndexViewModel model)
    {
      if (ModelState.IsValid)
      {
        try
        {
          int userID = int.Parse(HttpContext.Session.GetInt32("UserID").ToString());
          int departmentID = await _utils.PostUserIDGetDepartmentID(userID);
          model.MaterialRequisitions.DepartmentTypeID = departmentID;
          model.MaterialRequisitions.FinalApprovalID = 0;
          model.MaterialRequisitions.RequisitionStatusTypeID = 1;
          model.MaterialRequisitions.RequisitionDate = DateTime.Now;
          _appDBContext.ST_MaterialRequisitions.Add(model.MaterialRequisitions);
          //await _appDBContext.SaveChangesAsync();

          model.MaterialRequisitions.MaterialRequisitionDetails.RemoveAll(e => e.ItemID == 0);
          model.MaterialRequisitions.MaterialRequisitionDetails.RemoveAll(e => e.ItemID == null || e.ItemID == 0);

          foreach (var detail in model.MaterialRequisitions.MaterialRequisitionDetails)
          {
            detail.RequisitionID = model.MaterialRequisitions.RequisitionID;
            _appDBContext.ST_MaterialRequisitionDetails.Add(detail);
          }

          await _appDBContext.SaveChangesAsync();

          var RequisitionID = model.MaterialRequisitions.RequisitionID;

          var MaterialRequisitionStatus = new ST_MaterialRequisitionStatus
          {
            RequisitionID = RequisitionID,
            ActionDate = DateTime.Now,
            ActionID = HttpContext.Session.GetInt32("UserID") ?? default(int),
            ActionStatusTypeID = 1
          };

          _appDBContext.ST_MaterialRequisitionStatuss.Add(MaterialRequisitionStatus);
    
          await _appDBContext.SaveChangesAsync();

          
          if (RequisitionID > 0)
          {
            var processCount = await _appDBContext.CR_ProcessTypeApprovalSetups
                                .Where(pta => pta.ProcessTypeID > 0 && pta.ProcessTypeID == 20)
                                .CountAsync();

            if (processCount > 0)
            {
              
              var newProcessTypeApproval = new CR_ProcessTypeApproval
              {
                ProcessTypeID = 20,
                Notes = "Pending New Material Requisition",
                Date = DateTime.Now,
                EmployeeID = await _utils.PostUserIDGetEmployeeID(userID),
                UserID = HttpContext.Session.GetInt32("UserID") ?? default(int),
                TransactionID = RequisitionID
              };

              _appDBContext.CR_ProcessTypeApprovals.Add(newProcessTypeApproval);
              await _appDBContext.SaveChangesAsync();

              var nextApprovalSetup = await _appDBContext.CR_ProcessTypeApprovalSetups
                                          .Where(pta => pta.ProcessTypeID == 20 && pta.Rank == 1)
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
              model.MaterialRequisitions.FinalApprovalID = 1;
              model.MaterialRequisitions.RequisitionStatusTypeID = 2;
              _appDBContext.ST_MaterialRequisitions.Update(model.MaterialRequisitions);

              MaterialRequisitionStatus.ActionStatusTypeID = 2;
              _appDBContext.ST_MaterialRequisitionStatuss.Update(MaterialRequisitionStatus);

              await _appDBContext.SaveChangesAsync();
              TempData["SuccessMessage"] = " Material Requisition successfully. No process setup found,  Material Requisition Approved.";
              return Json(new { success = true, message = "No process setup found,  Material Requisition Approved." });
            }
          }
          TempData["SuccessMessage"] = " Material Requisition Created successfully. Continue to the Approval Process Setup for  Material Requisition Approved.";

          return Json(new { success = true });
        }
        catch (Exception ex)
        {
          return Json(new { success = false, message = "Error: " + ex.Message });
        }

      }

      ViewBag.ItemList = await _utils.GetItemList();
      ViewBag.ItemNameList = await _utils.GetItemList();

      if (model.MaterialRequisitions.MaterialRequisitionDetails == null || !model.MaterialRequisitions.MaterialRequisitionDetails.Any())
      {
        model.MaterialRequisitions.MaterialRequisitionDetails = new List<ST_MaterialRequisitionDetail> { new ST_MaterialRequisitionDetail() };
      }

      if (model.MaterialRequisitions == null)
      {
        model.MaterialRequisitions = new ST_MaterialRequisition();
      }

      return PartialView("~/Views/StoreManagement/StoreManagement/MaterialRequisition/AddMaterialRequisition.cshtml", model);
    }
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
      var MaterialRequisitions = await _appDBContext.ST_MaterialRequisitions
          .Include(v => v.MaterialRequisitionDetails)
          .FirstOrDefaultAsync(v => v.RequisitionID == id);

      if (MaterialRequisitions == null)
      {
        return NotFound();
      }

      // Check RequisitionStatusTypeID
      if (MaterialRequisitions.RequisitionStatusTypeID != 1)
      {
        TempData["ErrorMessage"] = "After approval, editing is not allowed.....";
        
      }

      MaterialRequisitions.MaterialRequisitionDetails.Add(new ST_MaterialRequisitionDetail()
      {
        RequisitionID = MaterialRequisitions.RequisitionID
      });

      var model = new MaterialRequisitionsIndexViewModel
      {
        MaterialRequisitions = MaterialRequisitions
      };

      ViewBag.ItemList = await _utils.GetItemList();
      ViewBag.ItemNameList = await _utils.GetItemList();

      return PartialView("~/Views/StoreManagement/StoreManagement/MaterialRequisition/EditMaterialRequisition.cshtml", model);
    }



    [HttpPost]
    public async Task<IActionResult> Edit(MaterialRequisitionsIndexViewModel MaterialRequisition)
    {
      if (ModelState.IsValid)
      {
        
        var existingMaterialRequisition = await _appDBContext.ST_MaterialRequisitions
            .Include(v => v.MaterialRequisitionDetails)
            .FirstOrDefaultAsync(v => v.RequisitionID == MaterialRequisition.MaterialRequisitions.RequisitionID);

        if (existingMaterialRequisition != null)
        {
          existingMaterialRequisition.Remarks = MaterialRequisition.MaterialRequisitions.Remarks;
  
          _appDBContext.Update(existingMaterialRequisition);
          await _appDBContext.SaveChangesAsync();

          var MaterialRequisitionDetailsToRemove = _appDBContext.ST_MaterialRequisitionDetails
          .Where(v => v.RequisitionID == MaterialRequisition.MaterialRequisitions.RequisitionID)
          .ToList();

          _appDBContext.ST_MaterialRequisitionDetails.RemoveRange(MaterialRequisitionDetailsToRemove);

          await _appDBContext.SaveChangesAsync();
          MaterialRequisition.MaterialRequisitions.MaterialRequisitionDetails.RemoveAll(e => e.ItemID == null || e.ItemID == 0);

          foreach (var detail in MaterialRequisition.MaterialRequisitions.MaterialRequisitionDetails)
          {
            detail.RequisitionID = MaterialRequisition.MaterialRequisitions.RequisitionID;
            _appDBContext.ST_MaterialRequisitionDetails.Add(detail);
          }

          await _appDBContext.SaveChangesAsync();

          return Json(new { success = true, message = "Received MaterialRequisition Edited successfully!" });
        }
        else
        {
          return NotFound();
        }
      }

      ViewBag.ItemList = await _utils.GetItemList();
      ViewBag.ItemNameList = await _utils.GetItemList();

      return PartialView("~/Views/StoreManagement/StoreManagement/MaterialRequisition/EditMaterialRequisition.cshtml", MaterialRequisition);
    }
    public async Task<IActionResult> PrintMaterialRequisition(int id)
    {
      var MaterialRequisitions = await _appDBContext.ST_MaterialRequisitions
         .Include(v => v.MaterialRequisitionDetails)
         .FirstOrDefaultAsync(v => v.RequisitionID == id);

      if (MaterialRequisitions == null)
      {
        return NotFound();
      }

      
      MaterialRequisitions.MaterialRequisitionDetails.Add(new ST_MaterialRequisitionDetail()
      {
        RequisitionID = MaterialRequisitions.RequisitionID
      });

      var model = new MaterialRequisitionsIndexViewModel
      {
        MaterialRequisitions = MaterialRequisitions
      };

      ViewBag.ItemList = await _utils.GetItemList();
      ViewBag.ItemNameList = await _utils.GetItemList();

      return View("~/Views/StoreManagement/StoreManagement/MaterialRequisition/PrintMaterialRequisition.cshtml", MaterialRequisitions);
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
  }
}

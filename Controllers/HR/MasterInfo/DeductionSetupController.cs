using Exampler_ERP.Models;
using Exampler_ERP.Models.Temp;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace Exampler_ERP.Controllers.HR.MasterInfo
{
   public class DeductionSetupController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;

    public DeductionSetupController(AppDBContext appDBContext, IConfiguration configuration, Utils utils)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
    }
    public async Task<IActionResult> Index(string searchDeductionTypeName)
    {
      // Query for Deduction Types with optional search filter
      var deductionTypesQuery = _appDBContext.Settings_DeductionTypes.AsQueryable();
      if (!string.IsNullOrEmpty(searchDeductionTypeName))
      {
        deductionTypesQuery = deductionTypesQuery
            .Where(dt => dt.DeductionTypeName.Contains(searchDeductionTypeName));
      }

      var deductionTypes = await deductionTypesQuery.ToListAsync();
      var deductionSetups = await _appDBContext.HR_DeductionSetups.ToListAsync();
      var viewModel = new DeductionSetupListViewModel
      {
        DeductionTypeWithRowCount = deductionTypes.Select(dt => new DeductionTypeWithRowCountViewModel
        {
          DeductionTypeID = dt.DeductionTypeID,
          DeductionTypeName = dt.DeductionTypeName,
          Class = deductionSetups.Where(ds => ds.DeductionTypeID == dt.DeductionTypeID)
                                   .Select(ds => ds.ClassID)
                                   .Distinct()
                                   .FirstOrDefault(),
          RowCount = deductionSetups.Count(ds => ds.DeductionTypeID == dt.DeductionTypeID)
        }).ToList()
      };

      if (!string.IsNullOrEmpty(searchDeductionTypeName) && viewModel.DeductionTypeWithRowCount.Count == 0)
      {
        TempData["ErrorMessage"] = "No Deduction Type found with the name '" + searchDeductionTypeName + "'. Please check the name and try again.";
      }

      return View("~/Views/HR/MasterInfo/DeductionSetup/DeductionSetup.cshtml", viewModel);
    }

    public async Task<IActionResult> DeductionSetup()
    {
      var DeductionSetups = await _appDBContext.HR_DeductionSetups.ToListAsync();
      return Ok(DeductionSetups);
    }
    // Add the Edit action
    public async Task<IActionResult> Edit(int id)
    {
      var classIDList =  await _utils.GetClassIDList();
      var deductionValueList = await _utils.GetDeductionValueList();

      var deductionSetups = await _appDBContext.HR_DeductionSetups
          .Where(ds => ds.DeductionTypeID == id)
          .ToListAsync();

      var selectedSalaryTypeIds = deductionSetups.Select(ds => ds.SalaryTypeID).ToList();
      var selectedDeductionValueIds = deductionSetups
          .OrderBy(ds => ds.SalaryTypeID)
          .Select(ds => ds.DeductionValueID)
          .ToList();

      var allSalaryTypes = await _appDBContext.Settings_SalaryTypes.ToListAsync();
      var salaryTypeDeductions = allSalaryTypes.Select(st => new SalaryTypeDeductionViewModel
      {
        SalaryTypeID = st.SalaryTypeID,
        SalaryTypeName = st.SalaryTypeName,
        IsSelected = selectedSalaryTypeIds.Contains(st.SalaryTypeID),
        DeductionValueID = selectedSalaryTypeIds.Contains(st.SalaryTypeID)
              ? selectedDeductionValueIds[selectedSalaryTypeIds.IndexOf(st.SalaryTypeID)]
              : 4 // Default value
      }).ToList();

      var viewModel = new DeductionSetupViewModel
      {
        DeductionTypeID = id,
        ClassID = deductionSetups.FirstOrDefault()?.ClassID ?? 1,
        SalaryTypeDeductions = salaryTypeDeductions
      };

      ViewBag.DeductionTypes = await _appDBContext.Settings_DeductionTypes.ToListAsync();
      ViewBag.ClassIDList = classIDList;
      ViewBag.DeductionValueList = deductionValueList;
      ViewBag.SalaryTypes = await _appDBContext.Settings_SalaryTypes.ToListAsync();
      ViewBag.salaryTypeList = _utils.GetSalaryTypeList();

      return PartialView("~/Views/HR/MasterInfo/DeductionSetup/EditDeductionSetup.cshtml", viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(DeductionSetupViewModel viewModel)
    {
      if (!ModelState.IsValid)
      {
        // Handle validation errors
        var classIDList = _utils.GetClassIDList();
        var deductionValueList = _utils.GetDeductionValueList();

        ViewBag.DeductionTypes = await _appDBContext.Settings_DeductionTypes.ToListAsync();
        ViewBag.ClassIDList = classIDList;
        ViewBag.DeductionValueList = deductionValueList;
        ViewBag.SalaryTypes = await _appDBContext.Settings_SalaryTypes.ToListAsync();
        ViewBag.salaryTypeList = _utils.GetSalaryTypeList();
        TempData["ErrorMessage"] = "Error creating Deduction setups. Please check the inputs.";
        return PartialView("~/Views/HR/MasterInfo/DeductionSetup/EditDeductionSetup.cshtml", viewModel);
      }

      var existingSetups = await _appDBContext.HR_DeductionSetups
          .Where(ds => ds.DeductionTypeID == viewModel.DeductionTypeID)
          .ToListAsync();

      _appDBContext.HR_DeductionSetups.RemoveRange(existingSetups);

      var newSetups = viewModel.SalaryTypeDeductions
          .Where(st => st.IsSelected)
          .Select(st => new HR_DeductionSetup
          {
            DeductionTypeID = viewModel.DeductionTypeID,
            ClassID = viewModel.ClassID,
            SalaryTypeID = st.SalaryTypeID,
            DeductionValueID = st.DeductionValueID
          }).ToList();

      _appDBContext.HR_DeductionSetups.AddRange(newSetups);
      await _appDBContext.SaveChangesAsync();
      TempData["SuccessMessage"] = "Deduction setup updated successfully.";
      return Json(new { success = true});
    }

    public async Task<IActionResult> Delete(int id)
    {
      try
      {
        var deductionSetups = await _appDBContext.HR_DeductionSetups
            .Where(ds => ds.DeductionTypeID == id)
            .ToListAsync();

        if (!deductionSetups.Any())
        {
          TempData["ErrorMessage"] = "Deduction setups not found.";
          return Json(new { success = false});
        }

        _appDBContext.HR_DeductionSetups.RemoveRange(deductionSetups);
        await _appDBContext.SaveChangesAsync();
        TempData["SuccessMessage"] = "Deduction setups deleted successfully.";
        return Json(new { success = true});
      }
      catch (Exception ex)
      {
        TempData["ErrorMessage"] = "Error Updating Deduction setups. Please check the inputs.";
        return Json(new { success = false});
      }
    }

    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      // Fetch deduction setups from the database
      var deductionSetups = await _appDBContext.HR_DeductionSetups
          .Include(ds => ds.DeductionType)
          .Include(ds => ds.SalaryType)
          .ToListAsync();

      // Fetch deduction value list asynchronously
      var deductionValueList = await _utils.GetDeductionValueList();

      // Convert the list to a dictionary for quick lookup
      var deductionValueDict = deductionValueList
          .Where(dv => int.TryParse(dv.Value, out _)) // Ensure Value is a valid integer
          .ToDictionary(dv => int.Parse(dv.Value), dv => dv.Text);

      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add("DeductionSetups");

        // Add headers
        worksheet.Cells["A1"].Value = "DeductionSetup ID";
        worksheet.Cells["B1"].Value = "Deduction Type Name";
        worksheet.Cells["C1"].Value = "Class";
        worksheet.Cells["D1"].Value = "Salary Type";
        worksheet.Cells["E1"].Value = "Deduction Value";

        // Add data
        for (int i = 0; i < deductionSetups.Count; i++)
        {
          var deductionSetup = deductionSetups[i];
          worksheet.Cells[i + 2, 1].Value = deductionSetup.DeductionSetupID;
          worksheet.Cells[i + 2, 2].Value = deductionSetup.DeductionType?.DeductionTypeName ?? "Unknown";
          worksheet.Cells[i + 2, 3].Value = deductionSetup.ClassID == 1 ? "Absent" : "Late";
          worksheet.Cells[i + 2, 4].Value = deductionSetup.SalaryType?.SalaryTypeName ?? "Unknown";

          // Lookup deduction value text from the dictionary
          string deductionValueText;
          if (deductionSetup.DeductionValueID == null || deductionSetup.DeductionValueID == 0)
          {
            deductionValueText = "Not Set";
          }
          else
          {
            deductionValueText = deductionValueDict.TryGetValue(deductionSetup.DeductionValueID.Value, out var text)
                ? text
                : "Unknown";
          }
          worksheet.Cells[i + 2, 5].Value = deductionValueText;
        }

        // Style the header row
        worksheet.Cells["A1:E1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        // Save to a MemoryStream and prepare for download
        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = $"DeductionSetups-{DateTime.Now:yyyyMMddHHmmssfff}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }



    public async Task<IActionResult> Print()
    {
      var deductionSetups = await _appDBContext.HR_DeductionSetups
          .Include(ds => ds.DeductionType)
          .Include(ds => ds.SalaryType)
          .ToListAsync();

      var deductionValueList = await _utils.GetDeductionValueList();

      if (deductionValueList != null)
      {
        var deductionValueDict = deductionValueList
            .Where(dv => !string.IsNullOrWhiteSpace(dv.Value) && int.TryParse(dv.Value, out _))
            .ToDictionary(dv => int.Parse(dv.Value), dv => dv.Text);

        ViewBag.DeductionValueDict = deductionValueDict;
      }
      else
      {
        ViewBag.DeductionValueDict = new Dictionary<int, string>(); // or handle appropriately
      }

      return View("~/Views/HR/MasterInfo/DeductionSetup/PrintDeductionSetups.cshtml", deductionSetups);
    }

  }
}

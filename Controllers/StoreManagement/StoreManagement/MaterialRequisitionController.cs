using Exampler_ERP.Models;
using Exampler_ERP.Models.Temp;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Exampler_ERP.Controllers.StoreManagement.StoreManagement
{
  public class MaterialRequisitionController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;

    public MaterialRequisitionController(AppDBContext appDBContext, IConfiguration configuration, Utils utils)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
    }
    public async Task<IActionResult> Index(string searchItemName)
    {
      if (HttpContext.Session.GetInt32("UserID") != null)
      {
        int EmployeeID = int.Parse(HttpContext.Session.GetInt32("UserID").ToString());
        var MaterialRequisitionsQuery = _appDBContext.ST_MaterialRequisitions
          .Include(v => v.MaterialRequisitionDetails)
          .Include(v => v.RequisitionStatusTypes)
          .Include(v => v.HR_Employees)
          .Where(v => v.HR_Employees.EmployeeID == EmployeeID);

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
      ViewBag.RequisitionStatus = await _utils.GetRequisitionStatus();
 
      ST_MaterialRequisition MaterialRequisitions = new ST_MaterialRequisition();
      MaterialRequisitions.MaterialRequisitionDetails.Add(new ST_MaterialRequisitionDetail() { RequisitionID = 1 });
      var model = new MaterialRequisitionsIndexViewModel
      {
        MaterialRequisitions = MaterialRequisitions
      };

      return PartialView("~/Views/StoreManagement/StoreManagement/MaterialRequisition/AddMaterialRequisition.cshtml", model);
    }
  }
}

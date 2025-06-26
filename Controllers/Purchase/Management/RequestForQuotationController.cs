using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Exampler_ERP.Controllers.Purchase.Management
{
  public class RequestForQuotationController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _conSTguration;
    private readonly Utils _utils;

    public RequestForQuotationController(AppDBContext appDBContext, IConfiguration conSTguration, Utils utils)
    {
      _appDBContext = appDBContext;
      _conSTguration = conSTguration;
      _utils = utils;
    }
    public async Task<IActionResult> Index(string searchItemName)
    {
      var RequestForQuotationsQuery = _appDBContext.PR_PurchaseRequests
          .Include(v => v.RequestStatusType)
          .AsQueryable();

      RequestForQuotationsQuery = RequestForQuotationsQuery
          .Where(v => v.RequestStatusTypeID == 2);

      var RequestForQuotations = await RequestForQuotationsQuery.ToListAsync();

      if (!string.IsNullOrEmpty(searchItemName) && RequestForQuotations.Count == 0)
      {
        TempData["ErrorMessage"] = $"No Purchase Request found with the name '{searchItemName}'. Please check the name and try again.";
      }

      return View("~/Views/Purchase/Management/RequestForQuotation/RequestForQuotation.cshtml", RequestForQuotations);
    }
  }
}

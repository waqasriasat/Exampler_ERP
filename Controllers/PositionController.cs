using Exampler_ERP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace Exampler_ERP.Controllers
{
  public class PositionController : Controller
  {
    private readonly AppDBContext _context;
    public PositionController(AppDBContext context)
    {
      _context = context;
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
      var controller = context.Controller as Controller;
      if (controller == null)
      {
        await next();
        return;
      }

      var getculture = await _context.CR_GlobalSettings
       .OrderBy(x => x.GlobalSettingID)
       .Select(x => x.CultureSetting)
       .FirstOrDefaultAsync() ?? "en-US";

      var position = await _context.Settings_LanguageSetups
          .Where(x => x.CultureCode == getculture)
          .Select(x => x.Position)
          .FirstOrDefaultAsync();

      controller.ViewBag.PositionDirection = position?.ToLower() ?? "ltr";

      ViewBag.PositionDirection = position?.ToLower() ?? "ltr"; 

      await next();
    }
  }
}

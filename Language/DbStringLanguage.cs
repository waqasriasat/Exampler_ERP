using Exampler_ERP.Models;
using Microsoft.Extensions.Localization;

namespace Exampler_ERP.Language
{
  public class DbStringLanguage : IStringLocalizer
  {
    private readonly IServiceProvider _serviceProvider;
    private readonly string _culture;

    public DbStringLanguage(IServiceProvider serviceProvider)
    {
      _serviceProvider = serviceProvider;
      _culture = Thread.CurrentThread.CurrentCulture.Name;
    }

    public LocalizedString this[string name]
    {
      get
      {
       
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDBContext>();

        var getculture = context.CR_GlobalSettings
       .OrderBy(x => x.GlobalSettingID) // OrderBy optional, par Top(1) ki tarah kaam karega
       .Select(x => x.CultureSetting)
       .FirstOrDefault() ?? "en-US";


        var value = context.CR_LanguageRecords
          .FirstOrDefault(x => x.LabelName == name && x.Culture == getculture)?.LabelValue;

        return new LocalizedString(name, value ?? name, resourceNotFound: value == null);
      }
    }

    public LocalizedString this[string name, params object[] arguments]
    {
      get
      {
        var format = this[name].Value;
        var formatted = string.Format(format, arguments);
        return new LocalizedString(name, formatted, false);
      }
    }

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
      using var scope = _serviceProvider.CreateScope();
      var context = scope.ServiceProvider.GetRequiredService<AppDBContext>();

      return context.CR_LanguageRecords
        .Where(x => x.Culture == _culture)
        .Select(x => new LocalizedString(x.LabelName, x.LabelValue, false))
        .ToList();
    }
  }
}

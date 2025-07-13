using Microsoft.Extensions.Localization;

namespace Exampler_ERP.Language
{
  public class DbStringLanguageFactory : IStringLocalizerFactory
  {
    private readonly IServiceProvider _serviceProvider;

    public DbStringLanguageFactory(IServiceProvider serviceProvider)
    {
      _serviceProvider = serviceProvider;
    }

    public IStringLocalizer Create(Type resourceSource)
    {
      return new DbStringLanguage(_serviceProvider);
    }

    public IStringLocalizer Create(string baseName, string location)
    {
      return new DbStringLanguage(_serviceProvider);
    }
  }
}

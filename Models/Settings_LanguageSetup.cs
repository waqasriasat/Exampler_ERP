using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class Settings_LanguageSetup
  {
    [Key]
    public int LanguageSetupID { get; set; } = 1;
    public string? CultureCode { get; set; }
    public string? CultureName { get; set; }
    public string? Position { get; set; }
  }
}

using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class CR_GlobalSetting
  {
    [Key]
    public int GlobalSettingID { get; set; } = 1;
    public string? CultureSetting { get; set; }
  }
}

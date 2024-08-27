using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class Settings_Religion
  {
    [Key]
    public int ReligionID { get; set; }
    public string? ReligionName { get; set; }
  }
}

using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class Settings_ReligionType
  {
    [Key]
    public int ReligionTypeID { get; set; }
    public string? ReligionTypeName { get; set; }
  }
}

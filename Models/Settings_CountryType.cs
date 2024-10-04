using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class Settings_CountryType
  {
    [Key]
    public int CountryTypeID { get; set; }
    public string? CountryTypeName { get; set; }
  }
}

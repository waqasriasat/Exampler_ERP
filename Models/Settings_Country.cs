using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class Settings_Country
  {
    [Key]
    public int CountryID { get; set; }
    public string? CountryName { get; set; }
  }
}

using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class Settings_SalaryType
  {
    [Key]
    public int SalaryTypeID { get; set; }
    public string? SalaryTypeName { get; set; }
    public int? DeleteYNID { get; set; }
    public int? ActiveYNID { get; set; }
  }
}

using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class Settings_AdditionalAllowanceType
  {
    [Key]
    public int AdditionalAllowanceTypeID { get; set; }
    public string? AdditionalAllowanceTypeName { get; set; }
    public int? DeleteYNID { get; set; }
    public int? ActiveYNID { get; set; }
  }
}

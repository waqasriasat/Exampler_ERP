using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class Settings_FixedDeductionType
  {
    [Key]
    public int FixedDeductionTypeID { get; set; }
    public string? FixedDeductionTypeName { get; set; }
    public int? DeleteYNID { get; set; }
    public int? ActiveYNID { get; set; }
  }
}

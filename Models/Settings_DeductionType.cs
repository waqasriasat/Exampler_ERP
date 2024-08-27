using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class Settings_DeductionType
  {
    [Key]
    public int DeductionTypeID { get; set; }
    public string? DeductionTypeName { get; set; }
    public int? DeleteYNID { get; set; }
    public int? ActiveYNID { get; set; }
  }
}

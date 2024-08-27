using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class Settings_DeductionValue
  {
    [Key]
    public int DeductionValueID { get; set; }
    public string? DeductionValueText { get; set; }
  }
}

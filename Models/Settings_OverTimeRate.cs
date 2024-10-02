using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class Settings_OverTimeRate
  {
    [Key]
    public int OverTimeRateTypeID { get; set; }
    public double? OverTimeRateValue { get; set; }
    public int? DeleteYNID { get; set; }
    public int? ActiveYNID { get; set; }
  }
}

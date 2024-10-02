using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class Settings_MonthType
  {
    [Key]
    public int MonthTypeID { get; set; }
    public string? MonthTypeName { get; set; }
  }
}

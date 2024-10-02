using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class Settings_OverTimeType
  {
    [Key]
    public int OverTimeTypeID { get; set; }
    public string? OverTimeTypeName { get; set; }
    public int? DeleteYNID { get; set; }
    public int? ActiveYNID { get; set; }
  }
}

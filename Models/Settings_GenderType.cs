using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class Settings_GenderType
  {
    [Key]
    public int GenderTypeID { get; set; }
    public string? GenderTypeName { get; set; }
  }
}

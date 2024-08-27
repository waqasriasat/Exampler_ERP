using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class Settings_DeductionClass
  {
    [Key]
    public int ClassID { get; set; }
    public string ClassName { get; set; }
  }
}

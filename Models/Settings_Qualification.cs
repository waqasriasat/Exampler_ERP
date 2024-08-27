using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class Settings_Qualification
  {
    [Key]
    public int QualificationID { get; set; }
    public string? QualificationName { get; set; }
    public int? DeleteYNID { get; set; }
    public int? ActiveYNID { get; set; }
  }
}

using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class Settings_QualificationType
  {
    [Key]
    public int QualificationTypeID { get; set; }
    public string? QualificationTypeName { get; set; }
    public int? DeleteYNID { get; set; }
    public int? ActiveYNID { get; set; }
  }
}

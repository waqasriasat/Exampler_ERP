using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exampler_ERP.Models
{
  public class Settings_SubQualificationType
  {
    [Key]
    public int SubQualificationTypeID { get; set; }
    public string? SubQualificationTypeName { get; set; }
    public int? DeleteYNID { get; set; }
    public int? ActiveYNID { get; set; }
    public int? QualificationTypeID { get; set; }

    [ForeignKey("QualificationTypeID")]
    public virtual Settings_QualificationType? QualificationType { get; set; }
  }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exampler_ERP.Models
{
  public class Settings_SubQualification
  {
    [Key]
    public int SubQualificationID { get; set; }
    public string? SubQualificationName { get; set; }
    public int? DeleteYNID { get; set; }
    public int? ActiveYNID { get; set; }
    public int? QualificationID { get; set; }

    [ForeignKey("QualificationID")]
    public virtual Settings_Qualification? Qualification { get; set; }
  }
}

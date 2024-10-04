using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exampler_ERP.Models
{
  public class Settings_SectionType
  {
    [Key]
    public int SectionTypeID { get; set; }
    public string? SectionTypeName { get; set; }
    public int? DeleteYNID { get; set; }
    public int? ActiveYNID { get; set; }
    public int? DepartmentTypeID { get; set; }

    [ForeignKey("DepartmentTypeID")]
    public virtual Settings_DepartmentType? DepartmentType { get; set; }
  }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exampler_ERP.Models
{
  public class Settings_DepartmentType
  {
    [Key]
    public int DepartmentTypeID { get; set; }
    public string? DepartmentTypeName { get; set; }
    public int? DeleteYNID { get; set; }
    public int? ActiveYNID { get; set; }
    public int? BranchTypeID { get; set; }

    [ForeignKey("BranchTypeID")]
    public virtual Settings_BranchType? BranchType { get; set; }
  }
}

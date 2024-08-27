using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exampler_ERP.Models
{
  public class Settings_Section
  {
    [Key]
    public int SectionID { get; set; }
    public string? SectionName { get; set; }
    public int? DeleteYNID { get; set; }
    public int? ActiveYNID { get; set; }
    public int? DepartmentID { get; set; }

    [ForeignKey("DepartmentID")]
    public virtual Settings_Department? Department { get; set; }
  }
}

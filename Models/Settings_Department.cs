using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exampler_ERP.Models
{
  public class Settings_Department
  {
    [Key]
    public int DepartmentID { get; set; }
    public string? DepartmentName { get; set; }
    public int? DeleteYNID { get; set; }
    public int? ActiveYNID { get; set; }
    public int? BranchID { get; set; }

    [ForeignKey("BranchID")]
    public virtual Settings_Branch? Branch { get; set; }
  }
}

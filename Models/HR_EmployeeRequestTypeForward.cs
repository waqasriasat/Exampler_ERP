using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class HR_EmployeeRequestTypeForward
  {
    [Key]
    public int EmployeeRequestTypeForwardID { get; set; }
    public int EmployeeRequestTypeID { get; set; }
    public int RoleTypeID { get; set; }

    [ForeignKey("EmployeeRequestTypeID")]
    public virtual Settings_EmployeeRequestType? EmployeeRequestType { get; set; }
    [ForeignKey("RoleTypeID")]
    public virtual Settings_RoleType? RoleType { get; set; }
  }
}

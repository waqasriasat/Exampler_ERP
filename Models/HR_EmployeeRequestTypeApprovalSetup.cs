using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class HR_EmployeeRequestTypeApprovalSetup
  {
    [Key]
    public int EmployeeRequestTypeApprovalSetupID { get; set; }
    public int EmployeeRequestTypeID { get; set; }
    [ForeignKey("EmployeeRequestTypeID")]
    public virtual Settings_EmployeeRequestType? EmployeeRequestType { get; set; }
    public int Rank { get; set; }
    public int RoleTypeID { get; set; }
    [ForeignKey("RoleTypeID")]
    public virtual Settings_RoleType? RoleType { get; set; }
  }
}

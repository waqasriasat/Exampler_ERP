using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exampler_ERP.Models
{
  public class CR_User
  {
    [Key]
    public int UserID { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public int? EmployeeID { get; set; }  

    //[ForeignKey("EmployeeID")]
    //public virtual Employee? Employee { get; set; }
    public int? LoginStatus { get; set; }
    public int? DeleteYNID { get; set; }
    public int? ActiveYNID { get; set; }
    public int RoleID { get; set; }

    [ForeignKey("RoleID")]
    public virtual Settings_Role? Role { get; set; }
    public int? FinalApprovalID { get; set; }
    public int? ApprovalProcessID { get; set; }
  }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exampler_ERP.Models
{
  public class CR_ProcessTypeApprovalSetup
  {
    [Key]
    public int ProcessTypeApprovalID { get; set; }
    public int ProcessTypeID { get; set; }
    [ForeignKey("ProcessTypeID")]
    public virtual Settings_ProcessType? ProcessType { get; set; }
    public int Rank { get; set; }
    public int RoleID { get; set; }
    [ForeignKey("RoleID")]
    public virtual Settings_Role? Role { get; set; }

  }
}

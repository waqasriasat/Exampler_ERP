using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exampler_ERP.Models
{
  public class CR_ProcessTypeForward
  {
    [Key]
    public int ProcessTypeForwardID { get; set; }
    public int ProcessTypeID { get; set; }
    public int RoleID { get; set; }

    [ForeignKey("ProcessTypeID")]
    public virtual Settings_ProcessType? ProcessType { get; set; }
    [ForeignKey("RoleID")]
    public virtual Settings_Role? Role { get; set; }
  }
}

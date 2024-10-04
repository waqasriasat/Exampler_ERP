using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exampler_ERP.Models
{
  public class CR_ProcessTypeForward
  {
    [Key]
    public int ProcessTypeForwardID { get; set; }
    public int ProcessTypeID { get; set; }
    public int RoleTypeID { get; set; }

    [ForeignKey("ProcessTypeID")]
    public virtual Settings_ProcessType? ProcessType { get; set; }
    [ForeignKey("RoleTypeID")]
    public virtual Settings_RoleType? RoleType { get; set; }
  }
}

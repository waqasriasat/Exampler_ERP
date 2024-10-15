using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exampler_ERP.Models
{
  public class CR_ProcessTypeApprovalDetailDoc
  {
    [Key]
    public int ProcessTypeApprovalDetailDocID { get; set; }
    public int ProcessTypeApprovalDetailID { get; set; }
    [ForeignKey("ProcessTypeApprovalDetailID")]
    public virtual CR_ProcessTypeApprovalDetail? CR_ProcessTypeApprovalDetail { get; set; }
    public byte[] Doc { get; set; } = null!;
    public string DocExt { get; set; } = null!;
    public string DocName { get; set; } = null!;

    
  }
}

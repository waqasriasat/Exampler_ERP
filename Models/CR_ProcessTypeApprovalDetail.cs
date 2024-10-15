using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exampler_ERP.Models
{
  public class CR_ProcessTypeApprovalDetail
  {
    [Key]
    public int ProcessTypeApprovalDetailID { get; set; }
    public int ProcessTypeApprovalID { get; set; }
    [ForeignKey("ProcessTypeApprovalID")]
    public virtual CR_ProcessTypeApproval? CR_ProcessTypeApproval { get; set; }
    public DateTime Date { get; set; }
    public int RoleID { get; set; }
   
    public int AppID { get; set; }
    public int AppUserID { get; set; }
    
    public string? Notes { get; set; }
    public int Rank { get; set; }

    public virtual List<CR_ProcessTypeApprovalDetailDoc> ProcessTypeApprovalDetailDoc { get; set; } = new List<CR_ProcessTypeApprovalDetailDoc>();
  }
}

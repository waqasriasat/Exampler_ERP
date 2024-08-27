namespace Exampler_ERP.Models
{
  public class CR_ProcessTypeApprovalDetailDoc
  {
    public int ApprovalProcessDetailID { get; set; }
    public byte[] Doc { get; set; } = null!;
    public string DocExt { get; set; } = null!;
    public string DocName { get; set; } = null!;

    // Navigation property
    public virtual CR_ProcessTypeApprovalDetail? ProcessTypeApprovalDetail { get; set; }
  }
}

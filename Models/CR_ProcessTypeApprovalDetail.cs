namespace Exampler_ERP.Models
{
  public class CR_ProcessTypeApprovalDetail
  {
    public int ApprovalProcessDetailID { get; set; }
    public int ApprovalProcessID { get; set; }
    public DateTime Date { get; set; }
    public int RoleID { get; set; }
    public int AppID { get; set; }
    public int AppUserID { get; set; }
    public bool IsDirect { get; set; }
    public string? Notes { get; set; }
    public int Rank { get; set; }

    // Navigation property
    public virtual CR_ProcessTypeApproval? ProcessTypeApproval { get; set; }
  }
}

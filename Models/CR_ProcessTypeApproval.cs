namespace Exampler_ERP.Models
{
  public class CR_ProcessTypeApproval
  {
    public int ApprovalProcessID { get; set; }
    public int ProcessTypeID { get; set; }
    public string? Notes { get; set; }
    public DateTime Date { get; set; }
    public int EmployeeID { get; set; }
    public string RequestDate { get; set; } = null!;
    public int UserID { get; set; }
    public int TransactionID { get; set; }
  }
}

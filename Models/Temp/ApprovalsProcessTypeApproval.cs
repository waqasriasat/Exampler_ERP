using System.ComponentModel.DataAnnotations.Schema;

namespace Exampler_ERP.Models.Temp
{
  public class ApprovalsProcessTypeApproval
  {
    public int ApprovalProcessID { get; set; }
    public int ProcessTypeID { get; set; }
    [ForeignKey("ProcessTypeID")]
    public virtual Settings_ProcessType? ProcessType { get; set; }
    public string? Notes { get; set; }
    public DateTime Date { get; set; }
    public int EmployeeID { get; set; }
    [ForeignKey("EmployeeID")]
    public virtual HR_Employee? Employee { get; set; }
    public string RequestDate { get; set; } = null!;
    public int UserID { get; set; }
    [ForeignKey("UserID")]
    public virtual CR_User? User { get; set; }
    public int TransactionID { get; set; }
  }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exampler_ERP.Models
{
  public class CR_ProcessTypeApproval
  {
   [Key]
    public int ProcessTypeApprovalID { get; set; }
    public int ProcessTypeID { get; set; }
    [ForeignKey("ProcessTypeID")]
    public virtual Settings_ProcessType? ProcessType { get; set; }
    public string? Notes { get; set; }
    public DateTime Date { get; set; }
    public int EmployeeID { get; set; }
    [ForeignKey("EmployeeID")]
    public virtual HR_Employee? Employee { get; set; }
    public int UserID { get; set; }
    [ForeignKey("UserID")]
    public virtual CR_User? User { get; set; }
    public int TransactionID { get; set; }
    public virtual List<CR_ProcessTypeApprovalDetail> ProcessTypeApprovalDetail { get; set; } = new List<CR_ProcessTypeApprovalDetail>();
  }
}

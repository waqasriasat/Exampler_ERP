using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class HR_EmployeeRequestTypeApproval
  {
    [Key]
    public int EmployeeRequestTypeApprovalID { get; set; }
    public int EmployeeRequestTypeID { get; set; }
    [ForeignKey("EmployeeRequestTypeID")]
    public virtual Settings_EmployeeRequestType? EmployeeRequestType { get; set; }
    public string? Notes { get; set; }
    public DateTime Date { get; set; }
    public int EmployeeID { get; set; }
    [ForeignKey("EmployeeID")]
    public virtual HR_Employee? Employee { get; set; }
   
    public virtual List<HR_EmployeeRequestTypeApprovalDetail?> EmployeeRequestTypeApprovalDetail { get; set; } = new List<HR_EmployeeRequestTypeApprovalDetail?>();
    public int? ProcessTypeApprovalID { get; set; } //didn't use in registration
    public int? FinalApprovalID { get; set; } //didn't use in registration
    public int? DeleteYNID { get; set; } //didn't use in registration
  }
}

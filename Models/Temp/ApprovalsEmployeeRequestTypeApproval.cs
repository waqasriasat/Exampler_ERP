using System.ComponentModel.DataAnnotations.Schema;

namespace Exampler_ERP.Models.Temp
{
  public class ApprovalsEmployeeRequestTypeApproval
  {
    public int EmployeeRequestTypeApprovalID { get; set; }
    public int EmployeeRequestTypeID { get; set; }
    [ForeignKey("EmployeeRequestTypeID")]
    public virtual Settings_EmployeeRequestType? EmployeeRequestType { get; set; }
    public string? Notes { get; set; }
    public DateTime Date { get; set; }
    public int EmployeeID { get; set; }
    [ForeignKey("EmployeeID")]
    public virtual HR_Employee? Employee { get; set; }
    public string RequestDate { get; set; } = null!;
  }
}

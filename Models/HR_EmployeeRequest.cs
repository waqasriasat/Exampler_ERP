using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class HR_EmployeeRequest
  {
    [Key]
    public int EmployeeRequestID { get; set; }
    public int EmployeeID { get; set; }
    [ForeignKey("EmployeeID")]
    public virtual HR_Employee? Employee { get; set; }

    [Required]
    public int EmployeeRequestTypeID { get; set; } 
    [ForeignKey("EmployeeRequestTypeID")]
    public virtual Settings_EmployeeRequestType? Settings_EmployeeRequestType { get; set; }

    [Required]
    public DateTime Date { get; set; } //  RequestDate
    [StringLength(1000)]
    public string Notes { get; set; }
    public int? FinalApprovalID { get; set; }
    public int? ApprovalProcessID { get; set; }
    public int? DeleteYNID { get; set; }
  }
}

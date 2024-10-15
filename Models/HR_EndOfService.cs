using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exampler_ERP.Models
{
  public class HR_EndOfService
  {
    [Key]
    public int EndOfServiceID { get; set; }
    public int EmployeeID { get; set; }
    [ForeignKey("EmployeeID")]
    public virtual HR_Employee? Employee { get; set; }
    public DateTime DateOfCompletionOfWork { get; set; }
    public int EndOfSerivceReasonTypeId { get; set; }
    [ForeignKey("EndOfSerivceReasonTypeId")]
    public virtual Settings_EndOfServiceReasonType? Settings_EndOfSerivceReasonType { get; set; }
    public int NumberofYears { get; set; }
    public int NumberofMonths { get; set; }
    public int NumberofDays { get; set; }
    public int TotalDays { get; set; }
    public float EndofServiceBenefit { get; set; }
    public int BalanceOfTheAnnualLeave { get; set; }
    public float AmountDueForTheLeave { get; set; }
    public float TotalEndOfServiceDues { get; set; }
    public int? DeleteYNID { get; set; }
    public int? FinalApprovalID { get; set; }
    public int? ProcessTypeApprovalID { get; set; }
  }
}

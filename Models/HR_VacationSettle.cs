using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exampler_ERP.Models
{
  public class HR_VacationSettle
  {
    [Key]
    public int VacationSettleID { get; set; }
    public int VacationID { get; set; }
    [ForeignKey("VacationID")]
    public virtual HR_Vacation? Vacation { get; set; }
    public int SettleDays { get; set; }
    public int SettleAmount { get; set; }
    public int? FinalApprovalID { get; set; } 
    public int? ApprovalProcessID { get; set; } 
  }
}

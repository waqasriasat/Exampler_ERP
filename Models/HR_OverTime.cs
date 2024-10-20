using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class HR_OverTime
  {
    [Key]
    public int OverTimeID { get; set; }
    public int OverTimeTypeID { get; set; }
    [ForeignKey("OverTimeTypeID")]
    public virtual Settings_OverTimeType? OverTimeType { get; set; }
    public int EmployeeID { get; set; }
    [ForeignKey("EmployeeID")]
    public virtual HR_Employee? Employee { get; set; }
    public int? MonthTypeID { get; set; }
    [ForeignKey("MonthTypeID")]
    public virtual Settings_MonthType? MonthType { get; set; }
    public int? Year { get; set; }
    public int? Days { get; set; }
    public int? Hours { get; set; }
    public int? OvertimeRateID { get; set; }
    [ForeignKey("OvertimeRateID")]
    public virtual Settings_OverTimeRate? OverTimeRate { get; set; }
    public float? Amount { get; set; }
    public int? DeleteYNID { get; set; }
    public int? FinalApprovalID { get; set; }
    public int? ProcessTypeApprovalID { get; set; }
    public int? PostedID { get; set; }
    public int? PayRollID { get; set; }
  }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exampler_ERP.Models
{
  public class HR_Contract
  {
    [Key]
    public int ContractID { get; set; }
    public int EmployeeID { get; set; }
    [ForeignKey("EmployeeID")]
    public virtual HR_Employee? Employee { get; set; }
    public DateTime IssueDate { get; set; }
    public int? ActiveID { get; set; }
    public int? DeleteYNID { get; set; }
    public int SalaryTypeID { get; set; }
    public int ContractTypeID { get; set; }
    [ForeignKey("ContractTypeID")]
    public virtual Settings_ContractType? Settings_ContractType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? VacationDays { get; set; }
    public int? DHours { get; set; }
    public int? DMinutes { get; set; }
    public int? FinalApprovalID { get; set; }
    public int? ApprovalProcessID { get; set; }
  }
}

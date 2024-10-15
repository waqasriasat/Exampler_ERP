using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class HR_EmployeeRequestTypeApprovalDetail
  {
    [Key]
    public int EmployeeRequestTypeApprovalDetailID { get; set; }
    public int EmployeeRequestTypeApprovalID { get; set; }
    [ForeignKey("EmployeeRequestTypeApprovalID")]
    public virtual HR_EmployeeRequestTypeApproval? HR_EmployeeRequestTypeApproval { get; set; }
    public DateTime Date { get; set; }
    public int RoleID { get; set; }

    public int AppID { get; set; }
    public int AppUserID { get; set; }

    public string? Notes { get; set; }
    public int Rank { get; set; }

    public virtual List<HR_EmployeeRequestTypeApprovalDetailDoc> EmployeeRequestTypeApprovalDetailDoc { get; set; } = new List<HR_EmployeeRequestTypeApprovalDetailDoc>();
  }
}

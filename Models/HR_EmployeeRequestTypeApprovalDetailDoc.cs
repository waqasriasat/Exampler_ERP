using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class HR_EmployeeRequestTypeApprovalDetailDoc
  {
    [Key]
    public int EmployeeRequestTypeApprovalDetailDocID { get; set; }
    public int EmployeeRequestTypeApprovalDetailID { get; set; }
    [ForeignKey("EmployeeRequestTypeApprovalDetailID")]
    public virtual HR_EmployeeRequestTypeApprovalDetail? HR_EmployeeRequestTypeApprovalDetail { get; set; }
    public byte[] Doc { get; set; } = null!;
    public string DocExt { get; set; } = null!;
    public string DocName { get; set; } = null!;
  }
}

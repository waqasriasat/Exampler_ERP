using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class PR_PurchaseRequest
  {
    [Key]
    public int PurchaseRequestID { get; set; }
    public DateTime PurchaseRequestDate { get; set; }
    public string? Remarks { get; set; }
    public int? FinalApprovalID { get; set; }
    public int? ProcessTypeApprovalID { get; set; }
  }
}

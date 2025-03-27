using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
    public int? RequestStatusTypeID { get; set; }
    [ForeignKey("RequestStatusTypeID")]
    public virtual Settings_RequestStatusType? RequestStatusTypes { get; private set; }

    public virtual List<PR_PurchaseRequestDetail> PurchaseRequestDetails { get; set; } = new List<PR_PurchaseRequestDetail>();
  }
}

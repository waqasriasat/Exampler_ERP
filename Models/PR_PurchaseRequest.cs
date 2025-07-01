using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class PR_PurchaseRequest
  {
    [Key]
    public int PurchaseRequestID { get; set; }
    public DateTime PurchaseRequestDate { get; set; }
    public string? Remarks { get; set; }
    public int ItemID { get; set; }
    [ForeignKey("ItemID")]
    public virtual ST_Item? Item { get; set; }
    public int UnitTypeID { get; set; }
    public int Quantity { get; set; }
    public int PriorityLevel { get; set; }
    public int? ProcurementQueueID { get; set; }
    public int? RequestStatusTypeID { get; set; }
    [ForeignKey("RequestStatusTypeID")]
    public virtual Settings_RequestStatusType? RequestStatusType { get; set; }
    public int? FinalApprovalID { get; set; }
    public int? ProcessTypeApprovalID { get; set; }

  }
}

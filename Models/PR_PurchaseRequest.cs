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
    public int? RequestForQuotationID { get; set; }
    public int? QuotationVendorID1 { get; set; }
    public int? QuotationVendorID2 { get; set; }
    public int? QuotationVendorID3 { get; set; }
    public int? DeliverdVendorID { get; set; }
    public int? QuotationID { get; set; }
    public int? DCNo { get; set; }
    public int? InvoiceNo { get; set; }
    public int? RequestStatusTypeID { get; set; }
    [ForeignKey("RequestStatusTypeID")]
    public virtual Settings_RequestStatusType? RequestStatusType { get; set; }
    public int? ProcurementQueueID { get; set; }
    public int? FinalApprovalID { get; set; }
    public int? ProcessTypeApprovalID { get; set; }

  }
}

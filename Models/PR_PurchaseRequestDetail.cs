using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class PR_PurchaseRequestDetail
  {
    [Key]
    public int PR_PurchaseRequestDetailID { get; set; }
    public int PR_PurchaseRequestID { get; set; }
    [ForeignKey("PR_PurchaseRequestID")]
    public virtual PR_PurchaseRequest? PR_PurchaseRequests { get; set; }
    public int ItemID { get; set; }
    [ForeignKey("ItemID")]
    public virtual ST_Item? Item { get; set; }
    public int UnitTypeID { get; set; }
    public int Quantity { get; set; }
    public int PriorityLevel { get; set; }
    public int? RequestStatusTypeID { get; set; }
    public int? RequestForQuotationID { get; set; }
    public int? QuotationVendorID1 { get; set; }
    public int? QuotationVendorID2 { get; set; }
    public int? QuotationVendorID3 { get; set; }
    public int? DeliverdVendorID { get; set; }
    public int? QuotationID { get; set; }
    public int? DCNo { get; set; }
    public int? InvoiceNo { get; set; }

  }
}

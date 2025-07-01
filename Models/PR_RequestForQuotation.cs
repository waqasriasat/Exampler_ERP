using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exampler_ERP.Models
{
  public class PR_RequestForQuotation
  {
    [Key]
    public int RequestForQuotationID { get; set; }
    public int PurchaseRequestID { get; set; }
    [ForeignKey("PurchaseRequestID")]
    public virtual PR_PurchaseRequest? PurchaseRequests { get; set; }
    public int? QuotationVendorID1 { get; set; }
    public int? QuotationVendorID2 { get; set; }
    public int? QuotationVendorID3 { get; set; }
  }
}

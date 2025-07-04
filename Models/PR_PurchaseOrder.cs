using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exampler_ERP.Models
{
  public class PR_PurchaseOrder
  {
    [Key]
    public int PurchaseOrderID { get; set; }
    public int PurchaseRequestID { get; set; }
    [ForeignKey("PurchaseRequestID")]
    public virtual PR_PurchaseRequest? PurchaseRequests { get; set; }
    public int? PONO { get; set; }
    public int? DCNo { get; set; }
    public int? InvoiceNo { get; set; }
  }
}

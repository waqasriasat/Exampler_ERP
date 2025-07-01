using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exampler_ERP.Models
{
  public class PR_CostComparison
  {
    [Key]
    public int CostComparisonID { get; set; }
    public int PurchaseRequestID { get; set; }
    [ForeignKey("PurchaseRequestID")]
    public virtual PR_PurchaseRequest? PurchaseRequests { get; set; }
    public int? DeliverdVendorID { get; set; }
    public int? QuotationID1 { get; set; }
    public int? QuotationID2 { get; set; }
    public int? QuotationID3 { get; set; }
  }
}

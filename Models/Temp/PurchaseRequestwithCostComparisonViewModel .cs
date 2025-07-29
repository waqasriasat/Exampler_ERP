using System.ComponentModel.DataAnnotations.Schema;

namespace Exampler_ERP.Models.Temp
{
  public class PurchaseRequestwithCostComparisonViewModel
  {
    public List<PR_PurchaseRequest> PurchaseRequests { get; set; }
    public List<PR_CostComparison> CostComparisons { get; set; }
  }
}

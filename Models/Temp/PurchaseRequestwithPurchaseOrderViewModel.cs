namespace Exampler_ERP.Models.Temp
{
  public class PurchaseRequestwithPurchaseOrderViewModel
  {
    public PR_PurchaseRequest PurchaseRequests { get; set; }
    public PR_RequestForQuotation RequestForQuotations { get; set; }
    public PR_CostComparison CostComparisons { get; set; }
    public Settings_RequestStatusType RequestStatusType { get; set; }
    public ST_Item Item { get; set; }
    public FI_Vendor Vendor { get; set; }
    public Settings_HeadofAccount_Five HeadofAccount_Five { get; set; }
  }
}

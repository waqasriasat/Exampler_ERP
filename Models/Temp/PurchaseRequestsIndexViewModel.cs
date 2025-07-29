using System.ComponentModel.DataAnnotations.Schema;

namespace Exampler_ERP.Models.Temp
{
  public class PurchaseRequestViewModel
  {
    public int ItemID { get; set; }
    public string? ItemName { get; set; }
    public int TotalRank { get; set; }

    [ForeignKey("ItemID")]
    public virtual ST_Item? Item { get; set; }
  }
  public class PurchaseRequestIndexViewModel
  {
    
    public PR_PurchaseRequest PurchaseRequests { get; set; } = new();
    //public virtual List<PR_PurchaseRequestDetail> @Localizer["lbl_Purchase"]RequestDetails { get; set; } = new List<PR_PurchaseRequestDetail>();
  }
  //public class MaterialReceivedListViewModel
  //{
  //  public List<ItemWithMaterialReceivedQuantityViewModel> ItemsWithMaterialReceivedQuantity { get; set; }
  //}

  //public class ItemWithMaterialReceivedQuantityViewModel
  //{
  //  public int ItemID { get; set; }
  //  public string? ItemName { get; set; }
  //  public int MaterialReceivedQuantity { get; set; }
  //}
  //public class MaterialReceivedsIndexViewModel
  //{
  //  public ST_MaterialReceived MaterialReceiveds { get; set; } = new();
  //  public List<ST_MaterialReceivedComponent> MaterialReceivedComponents { get; set; } = new List<ST_MaterialReceivedComponent>();

  //}
}

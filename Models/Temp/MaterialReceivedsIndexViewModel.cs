using System.ComponentModel.DataAnnotations.Schema;

namespace Exampler_ERP.Models.Temp
{
  public class MaterialReceivedViewModel
  {
    public int ItemID { get; set; }
    public string? ItemName { get; set; }
    public int TotalRank { get; set; }

    [ForeignKey("ItemID")]
    public virtual ST_Item? Item { get; set; }
  }
  public class MaterialReceivedIndexViewModel
  {
    public List<ST_Item> Items { get; set; }
    public List<ST_MaterialReceived> MaterialReceiveds { get; set; }
  }
  public class MaterialReceivedListViewModel
  {
    public List<ItemWithMaterialReceivedQuantityViewModel> ItemsWithMaterialReceivedQuantity { get; set; }
  }

  public class ItemWithMaterialReceivedQuantityViewModel
  {
    public int ItemID { get; set; }
    public string? ItemName { get; set; }
    public int MaterialReceivedQuantity { get; set; }
  }
  public class MaterialReceivedsIndexViewModel
  {
    public ST_MaterialReceived MaterialReceiveds { get; set; } = new();
    public List<ST_MaterialReceivedComponent> MaterialReceivedComponents { get; set; } = new List<ST_MaterialReceivedComponent>();

  }
}

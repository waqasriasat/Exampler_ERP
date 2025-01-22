using System.ComponentModel.DataAnnotations.Schema;

namespace Exampler_ERP.Models.Temp
{
  public class StockViewModel
  {
    public int ItemID { get; set; }
    public string? ItemName { get; set; }
    public int TotalRank { get; set; }

    [ForeignKey("ItemID")]
    public virtual ST_Item? Item { get; set; }
  }
  public class StockIndexViewModel
  {
    public List<ST_Item> Items { get; set; }
    public List<ST_Stock> Stocks { get; set; }
  }
  public class StockListViewModel
  {
    public List<ItemWithStockQuantityViewModel> ItemsWithStockQuantity { get; set; }
  }

  public class ItemWithStockQuantityViewModel
  {
    public int ItemID { get; set; }
    public string? ItemName { get; set; }
    public int StockQuantity { get; set; }
  }
  public class StocksIndexViewModel
  {
    public ST_Stock Stocks { get; set; } = new();

  }
}

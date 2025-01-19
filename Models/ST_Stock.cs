using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class ST_Stock
  {
    [Key]
    public int StockID { get; set; }
    public DateTime StockDate { get; set; }
    public int ItemID { get; set; }
    [ForeignKey("ItemID")]
    public virtual ST_Item? Item { get; set; }
    public int UnitTypeID { get; set; } // Unit of measurement (e.g., pieces, kg, liters)
    public int Quantity { get; set; }
    public int VendorID { get; set; }
    [ForeignKey("VendorID")]
    public virtual FI_Vendor? Vendor { get; set; }
    public string? LotNumber { get; set; }
    public string? PONo { get; set; }
    public string? GRNNo { get; set; }
    public string? DCNo { get; set; }
    public string? InvoiceNo { get; set; }
    public virtual List<ST_StockComponent> StockComponents { get; set; } = new List<ST_StockComponent>();
  }
}

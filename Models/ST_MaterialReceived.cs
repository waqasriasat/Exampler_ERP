using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class ST_MaterialReceived
  {
    [Key]
    public int MaterialReceivedID { get; set; }
    public DateTime MaterialReceivedDate { get; set; }
    public int ItemID { get; set; }
    [ForeignKey("ItemID")]
    public virtual ST_Item? Item { get; set; }
    public int UnitTypeID { get; set; } // Unit of measurement (e.g., pieces, kg, liters)
    public int Quantity { get; set; }
    public int VendorID { get; set; }
    [ForeignKey("VendorID")]
    public virtual FI_Vendor? Vendor { get; set; }
    public string? LotNumber { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public int? PONo { get; set; }
    public string? GRNNo { get; set; }
    public string? DCNo { get; set; }
    public string? InvoiceNo { get; set; }
    public int? PurchaseRequestID { get; set; }
   
    public virtual List<ST_MaterialReceivedComponent> MaterialReceivedComponents { get; set; } = new List<ST_MaterialReceivedComponent>();
  }
}

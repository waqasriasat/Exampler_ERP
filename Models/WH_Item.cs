using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exampler_ERP.Models
{
  public class WH_Item
  {
    [Key]
    public int ItemID { get; set; } // Unique identifier for the item
    public string ItemCode { get; set; } // Unique code for the item
    public string ItemName { get; set; } // Name of the item
    public int ItemCategoryTypeID { get; set; } // Identifier for the item's category
    [ForeignKey("ItemCategoryTypeID")]
    public virtual Settings_ItemCategoryType? ItemCategoryType { get; set; }
    public int ItemGroupTypeID { get; set; } // Identifier for the item's group
    public int UnitTypeID { get; set; } // Unit of measurement (e.g., pieces, kg, liters)
    [ForeignKey("UnitTypeID")]
    public virtual Settings_UnitType? UnitType { get; set; }
    public int ManufacturerTypeID { get; set; } // Unit of measurement (e.g., pieces, kg, liters)
    [ForeignKey("ManufacturerTypeID")]
    public virtual Settings_ManufacturerType? ManufacturerType { get; set; }
    public decimal MinQty { get; set; } // Minimum stock quantity threshold
    public decimal MaxQty { get; set; } // Maximum stock quantity threshold
    public decimal ReorderLevel { get; set; } // Quantity level at which reordering is triggered
    public string BinLocation { get; set; } // Location of the item within the warehouse (e.g., shelf, bin)
    public string Specification { get; set; } // Free-text specification or description
    public DateTime? ManufactureDate { get; set; } // Manufacturing date of the item (nullable)
    public DateTime? ExpiryDate { get; set; } // Expiry date of the item (nullable)
    public string BarCode { get; set; } // Barcode or SKU for the item
    public decimal OpeningBalance { get; set; } // Initial stock balance
    public int? FinalApprovalID { get; set; } //didn't use in registration
    public int? ProcessTypeApprovalID { get; set; } //didn't use in registration
  }
}

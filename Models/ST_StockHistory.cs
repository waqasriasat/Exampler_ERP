using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class ST_StockHistory
  {
    [Key]
    public int StockHistoryID { get; set; } // Unique identifier for each history entry

    public int StockID { get; set; } // Reference to the Stock table
    [ForeignKey("StockID")]
    public virtual ST_Stock? ST_Stocks { get; set; }
    public DateTime StockDate { get; set; }
    public int ItemID { get; set; }
    public int UnitTypeID { get; set; }
    public int Quantity { get; set; }
    public int VendorID { get; set; }
    public string? LotNumber { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public int? PONo { get; set; }
    public string? GRNNo { get; set; }
    public string? DCNo { get; set; }
    public string? InvoiceNo { get; set; }

    public string? ActionType { get; set; } // Action type (e.g., Added, Updated, Deleted)
    public DateTime? ActionDate { get; set; } // When the action was performed
    public int? ActionUserID { get; set; } // User who performed the action
  }
}

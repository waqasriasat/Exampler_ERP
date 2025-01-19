using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class ST_StockComponent
  {
    [Key]
    public int StockComponentID { get; set; } // Unique identifier for each history entry

    public int StockID { get; set; }
    [ForeignKey("StockID")]
    public virtual ST_Stock? Stocks { get; set; }
    public int ItemComponentTypeID { get; set; }
    public String? ItemComponentValue { get; set; }
    public ComponentDataType ItemComponentDataType { get; set; }
  }
  public enum ComponentDataType
  {
    Int = 1,
    Date = 2,
    String = 3,
    Decimal = 4
  }
}

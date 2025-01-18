using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class ST_StockComponent
  {
    [Key]
    public int StockComponentID { get; set; } // Unique identifier for each history entry

    public int StockID { get; set; } 
    public int ItemComponentTypeID { get; set; }
    public String? ItemComponentValue { get; set; }
  }
}

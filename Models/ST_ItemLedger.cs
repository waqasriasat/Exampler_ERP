using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class ST_ItemLedger
  {
    [Key]
    public int ItemLedgerID { get; set; }
    public DateTime ItemLedgerDate { get; set; }
    public int ItemID { get; set; }
    public int StockID { get; set; }
    public int IssuanceID { get; set; }
    public int StockIn { get; set; }
    public int StockOut { get; set; }

  }
}

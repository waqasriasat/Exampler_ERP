using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class PR_ProcurementQueue
  {
    [Key]
    public int ProcurementQueueID { get; set; }
    public DateTime ProcurementQueueDate { get; set; }
    public int ItemID { get; set; }
    [ForeignKey("ItemID")]
    public virtual ST_Item? Item { get; set; }
    public int UnitTypeID { get; set; }
    public int Quantity { get; set; }
    public int? ForwardYNID { get; set; }
  }
}

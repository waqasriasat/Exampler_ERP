using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exampler_ERP.Models
{
  public class ST_MaterialRequisitionDetail
  {
    [Key]
    public int RequisitionDetailID { get; set; }
    public int RequisitionID { get; set; }
    [ForeignKey("RequisitionID")]
    public virtual ST_MaterialRequisition? MaterialRequisitions { get; private set; }
    public int ItemID { get; set; }
    [ForeignKey("ItemID")]
    public virtual ST_Item? Items { get; private set; } 
    public decimal Quantity { get; set; } 
    public DateTime RequiredDate { get; set; }
  }
}

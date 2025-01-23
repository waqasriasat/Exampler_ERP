using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class ST_MaterialIssuanceDetail
  {
    [Key]
    public int IssuanceDetailID { get; set; }
    public int IssuanceID { get; set; }
    [ForeignKey("IssuanceID")]
    public virtual ST_MaterialIssuance? MaterialIssuances { get; private set; }
    public int ItemID { get; set; }
    [ForeignKey("ItemID")]
    public virtual ST_Item? Items { get; private set; }
    public int RequisitionQuantity { get; set; }
    public int IssuanceQuantity { get; set; }
    public int BalanceQuantity { get; set; }
    public int? IssuanceStatusTypeID { get; set; }
    [ForeignKey("IssuanceStatusTypeID")]
    public virtual Settings_IssuanceStatusType? IssuanceStatusTypes { get; private set; }
    public string? Remarks { get; set; }
  }
}

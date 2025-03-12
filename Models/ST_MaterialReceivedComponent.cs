using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class ST_MaterialReceivedComponent
  {
    [Key]
    public int MaterialReceivedComponentID { get; set; } // Unique identifier for each history entry

    public int MaterialReceivedID { get; set; }
    [ForeignKey("MaterialReceivedID")]
    public virtual ST_MaterialReceived? MaterialReceiveds { get; set; }
    public int ItemComponentTypeID { get; set; }
    [ForeignKey("ItemComponentTypeID")]
    public virtual Settings_ItemComponentType? ItemComponentTypes { get; set; }
    public String? ItemComponentValue { get; set; }
    public MaterialComponentDataType ItemComponentDataType { get; set; }
  }
  public enum MaterialComponentDataType
  {
    Int = 1,
    Date = 2,
    String = 3,
    Decimal = 4
  }
}

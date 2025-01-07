using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class Settings_UnitType
  {
    [Key]
    public int UnitTypeID { get; set; }
    public string UnitTypeCode { get; set; } // Unique code for the item
    public string UnitTypeName { get; set; }
    public int? DeleteYNID { get; set; }
    public int? ActiveYNID { get; set; }
  }
}

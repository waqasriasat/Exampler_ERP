using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class Settings_ItemComponentType
  {
    [Key]
    public int ItemComponentTypeID { get; set; }
    public string ItemComponentTypeName { get; set; }
    public int? DeleteYNID { get; set; }
    public int? ActiveYNID { get; set; }
  }
}

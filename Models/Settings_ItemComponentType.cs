using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exampler_ERP.Models
{
  public class Settings_ItemComponentType
  {
    [Key]
    public int ItemComponentTypeID { get; set; }
    public int ItemCategoryTypeID { get; set; }
    [ForeignKey("ItemCategoryTypeID")]
    public virtual Settings_ItemCategoryType? ItemCategoryType { get; set; }
    public string ItemComponentTypeName { get; set; }
    public int? DeleteYNID { get; set; }
    public int? ActiveYNID { get; set; }
  }
}

using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class Settings_ItemCategoryType
  {
    [Key]
    public int ItemCategoryTypeID { get; set; }
    public string ItemCategoryTypeName { get; set; }
    public int? DeleteYNID { get; set; }
    public int? ActiveYNID { get; set; }
  }
}

using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class Settings_ManufacturerType
  {
    [Key]
    public int ManufacturerTypeID { get; set; }
    public string ManufacturerTypeName { get; set; }
    public int? DeleteYNID { get; set; }
    public int? ActiveYNID { get; set; }
  }
}

using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class Settings_HolidayType
  {
    [Key]
    public int HolidayTypeID { get; set; }
    public string? HolidayTypeName { get; set; }
    public int? DeleteYNID { get; set; }
    public int? ActiveYNID { get; set; }
  }
}

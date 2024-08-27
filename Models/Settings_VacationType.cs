using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class Settings_VacationType
  {
    [Key]
    public int VacationTypeID { get; set; }
    public string? VacationTypeName { get; set; }
    public int? DeleteYNID { get; set; }
    public int? ActiveYNID { get; set; }
  }
}

using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class Settings_DesignationType
  {
    [Key]
    public int DesignationTypeID { get; set; }
    public string? DesignationTypeName { get; set; }
    public int? DeleteYNID { get; set; }
    public int? ActiveYNID { get; set; }
  }
}

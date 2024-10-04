using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class Settings_RoleType
  {
    [Key]
    public int RoleTypeID { get; set; }
    public string? RoleTypeName { get; set; }
    public int? DeleteYNID { get; set; }
    public int? ActiveYNID { get; set; }
  }
}

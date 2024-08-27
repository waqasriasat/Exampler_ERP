using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class Settings_Role
  {
    [Key]
    public int RoleID { get; set; }
    public string? RoleName { get; set; }
    public int? DeleteYNID { get; set; }
    public int? ActiveYNID { get; set; }
  }
}

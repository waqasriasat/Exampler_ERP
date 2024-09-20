using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class Settings_ActiveYNIDType
  {
    [Key]
    public int ActiveYNID { get; set; }
    public string? ActiveName { get; set; }
  }
}

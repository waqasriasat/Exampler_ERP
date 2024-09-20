using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class Settings_DeleteYNIDType
  {
    [Key]
    public int DeleteYNID { get; set; }
    public string? DeleteName { get; set; }
  }
}

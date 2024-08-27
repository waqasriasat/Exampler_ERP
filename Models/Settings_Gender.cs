using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class Settings_Gender
  {
    [Key]
    public int GenderID { get; set; }
    public string? GenderName { get; set; }
  }
}

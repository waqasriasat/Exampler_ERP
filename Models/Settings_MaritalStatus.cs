using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class Settings_MaritalStatus
  {
    [Key]
    public int MaritalStatusID { get; set; }
    public string? MaritalStatusName { get; set; }
  }
}

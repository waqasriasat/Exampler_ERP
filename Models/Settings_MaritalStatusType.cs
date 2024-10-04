using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class Settings_MaritalStatusType
  {
    [Key]
    public int MaritalStatusTypeID { get; set; }
    public string? MaritalStatusTypeName { get; set; }
  }
}

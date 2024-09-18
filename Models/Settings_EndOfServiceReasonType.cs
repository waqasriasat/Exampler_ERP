using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class Settings_EndOfServiceReasonType
  {
    [Key]
    public int EndOfServiceReasonTypeId { get; set; }
    public string? EndOfServiceReasonTypeName { get; set; }
  }
}

using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class Settings_RequestStatusType
  {
    [Key]
    public int RequestStatusTypeID { get; set; }
    public string? RequestStatusTypeName { get; set; }
    public int? DeleteYNID { get; set; }
    public int? ActiveYNID { get; set; }
  }
}

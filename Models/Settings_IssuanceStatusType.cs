using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class Settings_IssuanceStatusType
  {
    [Key]
    public int IssuanceStatusTypeID { get; set; }
    public string? IssuanceStatusTypeName { get; set; }
    public int? DeleteYNID { get; set; }
    public int? ActiveYNID { get; set; }
  }
}

using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class Settings_RequisitionStatusType
  {
    [Key]
    public int RequisitionStatusTypeID { get; set; }
    public string? RequisitionStatusTypeName { get; set; }
    public int? DeleteYNID { get; set; }
    public int? ActiveYNID { get; set; }
  }
}

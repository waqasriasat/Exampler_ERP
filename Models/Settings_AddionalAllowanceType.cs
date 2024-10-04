using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class Settings_AddionalAllowanceType
  {
    [Key]
    public int AddionalAllowanceTypeID { get; set; }
    public string? AddionalAllowanceTypeName { get; set; }
    public int? DeleteYNID { get; set; }
    public int? ActiveYNID { get; set; }
  }
}

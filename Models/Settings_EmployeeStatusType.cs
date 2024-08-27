using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class Settings_EmployeeStatusType
  {
    [Key]
    public int EmployeeStatusTypeID { get; set; }
    public string? EmployeeStatusTypeName { get; set; }
    public int? DeleteYNID { get; set; }
    public int? ActiveYNID { get; set; }
  }
}

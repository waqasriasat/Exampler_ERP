using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class Settings_EmployeeRequestType
  {
    [Key]
    public int EmployeeRequestTypeID { get; set; }
    public string? EmployeeRequestTypeName { get; set; }
    public int? DeleteYNID { get; set; }
    public int? ActiveYNID { get; set; }
  }
}

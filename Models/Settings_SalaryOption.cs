using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class Settings_SalaryOption
  {
    [Key]
    public int SalaryOptionId { get; set; }
    public string? SalaryOptionName { get; set; }
  }
}

using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class Settings_ContractType
  {
    [Key]
    public int ContractTypeId { get; set; }
    public string? ContractTypeName { get; set; }
  }
}

using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class Settings_VoucherType
  {
    [Key]
    public int VoucherTypeID { get; set; }
    public string? VoucherTypeName { get; set; }
    public string? VoucherNature { get; set; }
    public string? VoucherPrefix { get; set; }
    public int? DeleteYNID { get; set; }
    public int? ActiveYNID { get; set; }
    public int? VoucherNumber { get; set; }
  }
}

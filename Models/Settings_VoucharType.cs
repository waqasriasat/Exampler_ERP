using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class Settings_VoucharType
  {
    [Key]
    public int VoucharTypeID { get; set; }
    public string? VoucharTypeName { get; set; }
    public string? VoucharNature { get; set; }
    public string? VoucharPrefix { get; set; }
    public int? DeleteYNID { get; set; }
    public int? ActiveYNID { get; set; }
    public int? VoucharNumber { get; set; }
  }
}

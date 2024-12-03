using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class Settings_HOA_CategoryType
  {
    [Key]
    public int CategoryTypeID { get; set; }
    public string? CategoryTypeName { get; set; }
  }
}

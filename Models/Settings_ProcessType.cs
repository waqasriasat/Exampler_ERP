using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
    public class Settings_ProcessType
  {
    [Key]
    public int ProcessTypeID { get; set; }
      public string? ProcessTypeName { get; set; }
      public int? DeleteYNID { get; set; }
      public int? ActiveYNID { get; set; }
    }
}

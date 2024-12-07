using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class Settings_HeadofAccount_First
  {
    [Key]
    public int HeadofAccount_FirstID { get; set; } // Assuming an Id column for primary key
    public string? HeadofAccount_FirstName { get; set; }
    public int? DeleteYNID { get; set; }
    public int? ActiveYNID { get; set; }
  }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exampler_ERP.Models
{
  public class Settings_HeadofAccount_Second
  {
    [Key]
    public int HeadofAccount_SecondID { get; set; } // Assuming an Id column for primary key
    public string? HeadofAccount_SecondName { get; set; }
    public int HeadofAccount_FirstID { get; set; }
    [ForeignKey("HeadofAccount_FirstID")]
    public virtual Settings_HeadofAccount_First? HeadofAccount_First { get; set; }
    public int? DeleteYNID { get; set; }
    public int? ActiveYNID { get; set; }
  }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exampler_ERP.Models
{
  public class Settings_HeadofAccount_Third
  {
    [Key]
    public int HeadofAccount_ThirdID { get; set; } // Assuming an Id column for primary key
    public string? HeadofAccount_ThirdName { get; set; }
    public int HeadofAccount_SecondID { get; set; }
    [ForeignKey("HeadofAccount_SecondID")]
    public virtual Settings_HeadofAccount_Second? HeadofAccount_Second { get; set; }
    public int? DeleteYNID { get; set; }
    public int? ActiveYNID { get; set; }
  }
}

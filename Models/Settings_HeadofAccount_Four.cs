using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exampler_ERP.Models
{
  public class Settings_HeadofAccount_Four
  {
    [Key]
    public int HeadofAccount_FourID { get; set; } // Assuming an Id column for primary key
    public string? HeadofAccount_FourName { get; set; }
    public int HeadofAccount_ThirdID { get; set; }
    [ForeignKey("HeadofAccount_ThirdID")]
    public virtual Settings_HeadofAccount_Third? HeadofAccount_Third { get; set; }
  }
}

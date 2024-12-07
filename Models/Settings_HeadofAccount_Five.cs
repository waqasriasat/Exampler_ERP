using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exampler_ERP.Models
{
  public class Settings_HeadofAccount_Five
  {
    [Key]
    public int HeadofAccount_FiveID { get; set; } // Assuming an Id column for primary key
    public string? HeadofAccount_FiveName { get; set; }
    public int HeadofAccount_FourID { get; set; }
    [ForeignKey("HeadofAccount_FourID")]
    public virtual Settings_HeadofAccount_Four? HeadofAccount_Four { get; set; }
    public int CategoryTypeID { get; set; }
    [ForeignKey("CategoryTypeID")]
    public virtual Settings_HeadofAccount_CategoryType? HOA_CategoryType { get; set; }
    public int GroupTypeID { get; set; }//1=Balance Sheet, 2=Income sheet
    public int? DeleteYNID { get; set; }
    public int? ActiveYNID { get; set; }
  }
}

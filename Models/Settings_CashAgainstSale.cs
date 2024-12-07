using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class Settings_CashAgainstSale
  {
    [Key]
    public int CashAgainstSaleID { get; set; }
    public int? BranchTypeID { get; set; }

    [ForeignKey("BranchTypeID")]
    public virtual Settings_BranchType? BranchType { get; set; }
    public int? HeadofAccount_FiveID { get; set; }
    [ForeignKey("HeadofAccount_FiveID")]
    public virtual Settings_HeadofAccount_Five? HeadofAccount_Five { get; set; }

    public int? DeleteYNID { get; set; }
    public int? ActiveYNID { get; set; }
  }
}

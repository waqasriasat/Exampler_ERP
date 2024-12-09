using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exampler_ERP.Models
{
  public class FI_ChequeBook
  {
    [Key]
    public int ChequeBookID { get; set; } // Assuming an Id column for primary key
    public DateTime RegDate { get; set; }
    public int BankAccountID { get; set; }
    [ForeignKey("BankAccountID")]
    public virtual FI_Bank? BankAccount { get; set; }
    public int ChequeFrom { get; set; }
    public int ChequeTo { get; set; }
    public int TotalPages { get; set; }

    public int? ActiveYNID { get; set; }
    public int? DeleteYNID { get; set; }
    public virtual ICollection<FI_ChequeBookDetail> ChequeDetails { get; set; } = new List<FI_ChequeBookDetail>();
  }
}

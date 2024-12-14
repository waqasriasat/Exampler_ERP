using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exampler_ERP.Models
{
  public class FI_VoucharDetail
  {
    [Key]
    public int VoucharDetailID { get; set; } // Identity column
    public int VoucharID { get; set; } // Foreign key property
    [ForeignKey("VoucharID")]
    public virtual FI_Vouchar Vouchar { get; set; } // Navigation property
    public int HeadofAccount_FiveID { get; set; }
    [ForeignKey("HeadofAccount_FiveID")]
    public virtual Settings_HeadofAccount_Five? HeadofAccount_Five { get; set; }
    public string? DRCR { get; set; } // Debit/Credit
    public double? DrAmt { get; set; }
    public double? CrAmt { get; set; }
    public string? InstruAccount { get; set; }
    public DateTime? InstruDate { get; set; }
  }
}

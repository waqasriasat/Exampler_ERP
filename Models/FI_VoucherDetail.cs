using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exampler_ERP.Models
{
  public class FI_VoucherDetail
  {
    public FI_VoucherDetail()
    {
    }
    [Key]
    public int VoucherDetailID { get; set; } // Identity column
    public int VoucherID { get; set; } // Foreign key property
    [ForeignKey("VoucherID")]
    public virtual FI_Voucher? Voucher { get; private set; } // Navigation property
    public int? HeadofAccount_FiveID { get; set; }
    [ForeignKey("HeadofAccount_FiveID")]
    public virtual Settings_HeadofAccount_Five? HeadofAccount_Five { get; set; }
    public int? DRCR { get; set; } // 1=Debit/2=Credit
    public double? DrAmt { get; set; }
    public double? CrAmt { get; set; }
    
  }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.WebPages;

namespace Exampler_ERP.Models
{
  public class FI_Voucher
  {
    public FI_Voucher()
    {
    }
    [Key]
    public int VoucherID { get; set; } // Identity column
    public DateTime? VoucherDate { get; set; }
    public int VoucherTypeID { get; set; }
    [ForeignKey("VoucherTypeID")]
    public virtual Settings_VoucherType? VoucherType { get; set; }
    public string? VoucherNo { get; set; }
    public string? PayeeName { get; set; }
    public string? Description { get; set; }
    public string? PONo { get; set; }
    public string? GRNNo { get; set; }
    public string? DCNo { get; set; }
    public string? InvoiceNo { get; set; }
    public string? PVNo { get; set; }
    public string? VStatus { get; set; }
    public int? FinalApprovalID { get; set; } //didn't use in registration
    public int? ProcessTypeApprovalID { get; set; } //didn't use in registration
    public virtual List<FI_VoucherDetail> VoucherDetails { get; set; } = new List<FI_VoucherDetail>();
   
  }
}

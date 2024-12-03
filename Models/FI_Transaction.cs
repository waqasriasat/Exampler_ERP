using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exampler_ERP.Models
{
  public class FI_Vouchar
  {
    [Key]
    public int VoucharID { get; set; } // Identity column
    public DateTime VoucharDate { get; set; }
    public int VoucharTypeID { get; set; }
    [ForeignKey("VoucharTypeID")]
    public virtual Settings_VoucharType? VoucharType { get; set; }
    public string? VoucharNo { get; set; }
    public int HeadofAccount_FiveID { get; set; }
    [ForeignKey("HeadofAccount_FiveID")]
    public virtual Settings_HeadofAccount_Five? HeadofAccount_Five { get; set; }
    public string? PayeeName { get; set; }
    public string? DRCR { get; set; } // Debit/Credit
    public decimal DrAmt { get; set; }
    public decimal CrAmt { get; set; }
    public string? InstruType { get; set; } // Cash/Caheque
    public string? InstruAccount { get; set; }
    public DateTime? InstruDate { get; set; }
    public string? InstruNumber { get; set; } // ChequeNo
    public string? Description { get; set; }
    public string? PONo { get; set; }
    public string? GRNNo { get; set; }
    public string? DCNo { get; set; }
    public string? InvoiceNo { get; set; }
    public string? PVNo { get; set; }
    public string? VStatus { get; set; }
  }
}

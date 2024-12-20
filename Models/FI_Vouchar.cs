using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.WebPages;

namespace Exampler_ERP.Models
{
  public class FI_Vouchar
  {
    public FI_Vouchar()
    {
    }
    [Key]
    public int VoucharID { get; set; } // Identity column
    public DateTime VoucharDate { get; set; }
    public int VoucharTypeID { get; set; }
    [ForeignKey("VoucharTypeID")]
    public virtual Settings_VoucharType? VoucharType { get; set; }
    public string? VoucharNo { get; set; }
    public string? PayeeName { get; set; }
    public string? Description { get; set; }
    public string? PONo { get; set; }
    public string? GRNNo { get; set; }
    public string? DCNo { get; set; }
    public string? InvoiceNo { get; set; }
    public string? PVNo { get; set; }
    public string? VStatus { get; set; }
    public virtual List<FI_VoucharDetail> VoucharDetails { get; set; } = new List<FI_VoucharDetail>();
   
  }
}

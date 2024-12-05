using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class FI_Vendor
  {
    [Key]
    public int VendorID { get; set; }
    public string? VendorName { get; set; }
    public string? PayeeName { get; set; }
    public string? Cell { get; set; }
    public string? Phone { get; set; }
    public string? PersonName { get; set; }
    public string? Fax { get; set; }
    public bool Filer { get; set; }
    public string? STN { get; set; }
    public double? STNRate { get; set; }
    public string? NTN { get; set; }
    public double? NTNRate { get; set; }
    public int ICOA_ID { get; set; }
    public int SCOA_ID { get; set; }
    public string? Province { get; set; }
    public string? Federal { get; set; }
    public string? PaymentSec { get; set; }
    public string? Address { get; set; }
  }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exampler_ERP.Models
{
  public class FI_Vendor
  {
    [Key]
    public int VendorID { get; set; }
    public int? HeadofAccount_FiveID { get; set; }
    [ForeignKey("HeadofAccount_FiveID")]
    public virtual Settings_HeadofAccount_Five? HeadofAccount_Five { get; set; }
    public string? PayeeName { get; set; }
    public string? Cell { get; set; }
    public string? Phone { get; set; }
    public string? PersonName { get; set; }
    public string? Fax { get; set; }
    public int? Filer { get; set; }
    public string? STN { get; set; }
    public int? SaleTax_ID { get; set; }
    public double? STNRate { get; set; }
    public string? NTN { get; set; }
    public int? IncomTaxWithHoding_ID { get; set; }
    public double? NTNRate { get; set; }
    public string? Province { get; set; }
    public string? Federal { get; set; }
    public string? PaymentSec { get; set; }
    public string? Address { get; set; }

    public int? ActiveYNID { get; set; }
    public int? DeleteYNID { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
  }
}

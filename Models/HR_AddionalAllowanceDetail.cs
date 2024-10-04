using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class HR_AddionalAllowanceDetail
  {
    [Key]
    public int AddionalAllowanceDetailID { get; set; }
    public int? AddionalAllowanceID { get; set; }
    [ForeignKey("AddionalAllowanceID")]
    public virtual HR_AddionalAllowance? AddionalAllowance { get; set; }
    public int? AddionalAllowanceTypeID { get; set; }
    [ForeignKey("AddionalAllowanceTypeID")]
    public virtual Settings_AddionalAllowanceType? AddionalAllowanceType { get; set; }
    public double? AddionalAllowanceAmount { get; set; }
  }
}

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class HR_Holiday
  {
    [Key]
    public int HolidayID { get; set; }
    public int HolidayTypeID { get; set; }
    [ForeignKey("HolidayTypeID")]
    public virtual Settings_HolidayType? HolidayType { get; set; }
    public DateTime HolidayDate { get; set; }
    public int? FinalApprovalID { get; set; } //didn't use in registration
    public int? ProcessTypeApprovalID { get; set; } //didn't use in registration
    public int? ActiveYNID { get; set; } //didn't use in registration
    public int? DeleteYNID { get; set; } //didn't use in registration
  }
}

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class CR_ThumbAttendance
  {
    [Key]
    public int AttenanceID { get; set; }
    [Required]
    public int EmployeeID { get; set; }
    [ForeignKey("EmployeeID")]
    public virtual HR_Employee? Employee { get; set; }
    public DateTime MarkDate { get; set; }
    public int? MarkSourceID { get; set; } //1="in" and 2="out"
    public DateTime InTime { get; set; }
    public DateTime OutTime { get; set; }
    public int? DHours { get; set; }
    public int? DMinutes { get; set; }
  }
}

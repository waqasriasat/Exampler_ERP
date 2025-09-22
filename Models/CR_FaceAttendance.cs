using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class CR_FaceAttendance
  {
    [Key]
    public int FaceAttendanceID { get; set; }
    [Required]
    public int EmployeeID { get; set; }
    [ForeignKey("EmployeeID")]
    public virtual HR_Employee? Employee { get; set; }
    public DateTime MarkDate { get; set; }
    public int? MarkSourceID { get; set; } //1="in" and 2="out"
    public DateTime InTime { get; set; }
    public byte[]? InPicture { get; set; }
    public DateTime OutTime { get; set; }
    public byte[]? OutPicture { get; set; }
    public int? DHours { get; set; }
    public int? DMinutes { get; set; }
    public int? DeleteYNID { get; set;}
    public string? InDevice_SerialNo { get; set; }
    public int InPunchType { get; set; } //1=thumb, 15=face, 4=card
    public string? OutDevice_SerialNo { get; set; }
    public int OutPunchType { get; set; } //1=thumb, 15=face, 4=card
  }
}

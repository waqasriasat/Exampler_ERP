using System.ComponentModel.DataAnnotations.Schema;

namespace Exampler_ERP.Models.Temp
{
  public class FaceAttendanceForwardingViewModel
  {
    public int EmployeeID { get; set; }
    [ForeignKey("EmployeeID")]
    public virtual HR_Employee? Employee { get; set; }
    public DateTime MarkDate { get; set; }
    public string InTime { get; set; }
    public string OutTime { get; set; }
    public Int32 DHours { get; set; } // Changed to decimal
    public Int32 DMinutes { get; set; } // Changed to decimal
    public string FromDutyTime { get; set; }
    public string ToDutyTime { get; set; }
    public decimal LateComingGraceTime { get; set; } // Changed to decimal
    public decimal EarlyGoingGraceTime { get; set; } // Changed to decimal
    public string? PerDaySalary { get; set; }
    public string? LateComingDeduction { get; set; }
    public string? EarlyGoingDeduction { get; set; }
    public decimal EarlyComingGraceTime { get; set; } // Changed to decimal
    public decimal LateGoingGraceTime { get; set; } // Changed to decimal
    public string EarlyComingAmount { get; set; }
    public string LateGoingAmount { get; set; }
  }
}

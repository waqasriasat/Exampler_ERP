using Exampler_ERP.Models.Temp;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exampler_ERP.Models
{
  public class HR_Vacation
  {
    [Key]
    public int VacationID { get; set; }

    public int EmployeeID { get; set; }
    [ForeignKey("EmployeeID")]
    public virtual HR_Employee? Employee { get; set; }

    [Required]
    public int VacationTypeID { get; set; } // e.g., Annual Leave, Sick Leave, etc.
    [ForeignKey("VacationTypeID")]
    public virtual Settings_VacationType? Settings_VacationType { get; set; }

    [Required]
    public DateTime StartDate { get; set; } // Leave start date

    [Required]
    public DateTime EndDate { get; set; } // Leave end date

    [Range(0, int.MaxValue)]
    public int TotalDays { get; set; } // Total leave days, automatically calculated
   
    [StringLength(1000)]
    public string Reason { get; set; } // Reason for leave (optional)

    [Required]
    public DateTime Date { get; set; } // Date when the leave was applied

    public int? FinalApprovalID { get; set; }
    public int? ApprovalProcessID { get; set; }
  }
}

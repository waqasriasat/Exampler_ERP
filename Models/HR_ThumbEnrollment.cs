using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class HR_ThumbEnrollment
  {
    [Key]
    public int ThumbID { get; set; }

    [Required]
    public int EmployeeID { get; set; }

    [ForeignKey("EmployeeID")]
    public virtual HR_Employee Employee { get; set; }

    [Required]
    public byte[] ThumbImpression { get; set; } // Store thumb impression as binary data

    [Required]
    public string ThumbFormat { get; set; } // Format of the thumb image or data (e.g., 'ISO', 'ANSI', etc.)

    public DateTime EnrollmentDate { get; set; } // Date when the thumb was recorded
    public int? ActiveYNID { get; set; }
    public int? DeleteYNID { get; set; }
  }
}

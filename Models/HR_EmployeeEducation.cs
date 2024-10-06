using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exampler_ERP.Models
{
  public class HR_EmployeeEducation
  {
    [Key]
    public int EducationID { get; set; }  // Primary Key

    public int EmployeeID { get; set; }   // Foreign Key referencing the Employee
    [ForeignKey("EmployeeID")]
    public virtual HR_Employee? Employee { get; set; }
    public int? QualificationTypeID { get; set; }  // Education title (degree name)
    [ForeignKey("QualificationTypeID")]
    public virtual Settings_QualificationType? QualificationType { get; set; }
    public string InstituteName { get; set; }  // Name of the educational institute

    public string City { get; set; }  // City where the institute is located

    public int? CountryTypeID { get; set; }  // Country of the institute
    [ForeignKey("CountryTypeID")]
    public virtual Settings_CountryType? CountryType { get; set; }

    public int StartMonth { get; set; }  // Start month of education (1-12)

    public int StartYear { get; set; }   // Start year of education

    public int EndMonth { get; set; }    // End month of education (1-12)

    public int EndYear { get; set; }     // End year of education

    public byte[]? DocImage { get; set; }  // PDF document stored as binary data

    public string DocContent { get; set; } = "application/pdf";  // Content type for PDF

    public string DocExt { get; set; } = ".pdf";

    public virtual Settings_MonthType? MonthType { get; set; }
  }
}

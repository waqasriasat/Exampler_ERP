using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exampler_ERP.Models
{
  public class HR_Applicant
  {
    [Key]
    public int ApplicantID { get; set; }
    public DateTime? ApplyDate { get; set; }
    public byte[]? Picture { get; set; }
    public string? FirstName { get; set; }
    public string? FatherName { get; set; }
    public string? FamilyName { get; set; }
    public int? Religion { get; set; }
    public int? Sex { get; set; }
    public int? MaritalStatus { get; set; }
    public int? NoOfChildren { get; set; }
    public DateTime? DOB { get; set; }
    public string? PlaceOfBirth { get; set; }
    public string? Address { get; set; }
    public string? Phone1 { get; set; }
    public string? Phone2 { get; set; }
    public string? Fax { get; set; }
    public string? Email { get; set; }
    public string? POBox { get; set; }
    public string? Mobile { get; set; }
    public string? Whatsapp { get; set; }
    public string? IDNumber { get; set; }
    public string? IDPlaceOfIssue { get; set; }
    public string? PassportNumber { get; set; }
    public string? PassportPlaceOfIssue { get; set; }
    public int? CountryID { get; set; }
    public int? BranchID { get; set; }
    [ForeignKey("BranchID")]
    public virtual Settings_Branch? Branch { get; set; }
    public int? FinalApprovalID { get; set; }
    public int? ApprovalProcesslID { get; set; }
    public int? EducationTitleID { get; set; }
    public string? ForthePost { get; set; }
    public int? ExprienceNumberofYears { get; set; }

  }
}

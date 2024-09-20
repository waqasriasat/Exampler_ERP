using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exampler_ERP.Models
{
  public class HR_Employee
  {
    [Key]
    public int EmployeeID { get; set; } // generate unique id for each employee automatically
    public DateTime? HireDate { get; set; }
    public string? EmployeeCode { get; set; }
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
    public DateTime? IDExpiryDate { get; set; }
    public DateTime? IDIssueDate { get; set; }
    public string? PassportNumber { get; set; }
    public string? PassportPlaceOfIssue { get; set; }
    public DateTime? PassportExpiryDate { get; set; }
    public DateTime? PassportIssueDate { get; set; }
    public int? ReportToID { get; set; } //didn't use in registration
    public int? EmployeeStatusTypeID { get; set; } //didn't use in registration
    [ForeignKey("EmployeeStatusTypeID")]
    public virtual Settings_EmployeeStatusType? EmployeeStatusType { get; set; }
    public int? CountryID { get; set; }

    public int? DesignationID { get; set; }
    [ForeignKey("DesignationID")]
    public virtual Settings_Designation? Designation { get; set; }

    public int? DepartmentID { get; set; }
    [ForeignKey("DepartmentID")]
    public virtual Settings_Department? Department { get; set; }

    public int? BranchID { get; set; }
    [ForeignKey("BranchID")]
    public virtual Settings_Branch? Branch { get; set; }

    public int? ApplicantID { get; set; } //didn't use in registration
    public int? FinalApprovalID { get; set; } //didn't use in registration
    public int? ApprovalProcessID { get; set; } //didn't use in registration
    public int? ActiveYNID { get; set; } //didn't use in registration
    public int? DeleteYNID { get; set; } //didn't use in registration
  }
}

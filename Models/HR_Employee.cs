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

    public int? DesignationTypeID { get; set; }
    [ForeignKey("DesignationTypeID")]
    public virtual Settings_DesignationType? DesignationType { get; set; }

    public int? DepartmentTypeID { get; set; }
    [ForeignKey("DepartmentTypeID")]
    public virtual Settings_DepartmentType? DepartmentType { get; set; }

    public int? BranchTypeID { get; set; }
    [ForeignKey("BranchTypeID")]
    public virtual Settings_BranchType? BranchType { get; set; }

    public int? ApplicantID { get; set; } //didn't use in registration
    public int? FinalApprovalID { get; set; } //didn't use in registration
    public int? ProcessTypeApprovalID { get; set; } //didn't use in registration
    public int? ActiveYNID { get; set; } //didn't use in registration
    public int? DeleteYNID { get; set; } //didn't use in registration
    public string UserName { get; set; }
    public string Password { get; set; }
    public int? FromDutyTime { get; set; }
    public int? ToDutyTime { get; set; }
  }
}

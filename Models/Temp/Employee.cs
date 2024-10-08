namespace Exampler_ERP.Models.Temp
{
  public class Employee
  {
  }
  public class EmployeeListViewModel
  {
    public List<EmployeeCountViewModel> EmployeeCount { get; set; }
  }

  public class EmployeeCountViewModel
  {
    public int EmployeeID { get; set; }
    public string? EmployeeName { get; set; }
    public int? TypeCount { get; set; }
  }
  public class EmployeeJoiningListViewModel
  {
    public List<EmployeeJoiningViewModel> EmployeeJoining { get; set; }
  }
  public class EmployeeJoiningViewModel
  {
    public int EmployeeID { get; set; }
    public string? EmployeeName { get; set; }
    public DateTime? JoiningDate { get; set; }
  }
  public class EmployeeBankAccountListViewModel
  {
    public List<EmployeeBankAccountViewModel> EmployeeBankAccount { get; set; }
  }
  public class EmployeeBankAccountViewModel
  {
    public int EmployeeID { get; set; }
    public string? EmployeeName { get; set; }
    public string? BankName { get; set; }
  }
  public class EmployeeLeaveBlanceListViewModel
  {
    public List<EmployeeLeaveBlanceViewModel> EmployeeLeaveBlance { get; set; }
  }
  public class EmployeeLeaveBlanceViewModel
  {
    public int EmployeeID { get; set; }
    public string? EmployeeName { get; set; }
    public int? TotalBlanceLeave { get; set; }
  }
  public class EmployeeCardPrintListViewModel
  {
    public List<EmployeeCardPrintViewModel> EmployeeCardPrint { get; set; }
  }
  public class EmployeeCardPrintViewModel
  {
    public int EmployeeID { get; set; }
    public string? EmployeeName { get; set; }
  }
  public class UpdateReportToViewModel
  {
    public int EmployeeID { get; set; }
    public int? ReportToID { get; set; }
    
  }
  public class EmployeeEducationViewModel
  {
    public HR_EmployeeEducation NewEducation { get; set; }
    public List<HR_EmployeeEducation>? EmployeeEducations { get; set; }
  }
  public class EmployeeExperienceViewModel
  {
    public HR_EmployeeExperience NewExperience { get; set; }
    public List<HR_EmployeeExperience>? EmployeeExperiences { get; set; }
  }
  public class EmployeeBioDataViewModel
  {
    public HR_Employee Employee { get; set; }
    public List<HR_EmployeeEducation> Educations { get; set; }
    public List<HR_EmployeeExperience> Experiences { get; set; }
  }
}

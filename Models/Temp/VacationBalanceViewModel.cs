namespace Exampler_ERP.Models.Temp
{
  public class VacationBalanceViewModel
  {
    public int EmployeeID { get; set; }
    public string EmployeeName { get; set; }
    public DateTime? StartDate { get; set; } // Make nullable if dates can be null
    public DateTime? EndDate { get; set; } // Make nullable if dates can be null
    public int? YearlyVacation { get; set; }
    public double? TotalVacation { get; set; }
    public double? VacationBalance { get; set; }
  }
  public class VacationReportViewModel
  {
    public int EmployeeID { get; set; }
    public int VacationID { get; set; }
    public string EmployeeName { get; set; }
    public DateTime? StartDate { get; set; } // Make nullable if dates can be null
    public DateTime? EndDate { get; set; } // Make nullable if dates can be null
    public int? TotalDays { get; set; }
    public string? TypeOfVacation { get; set; }
   
  }
}

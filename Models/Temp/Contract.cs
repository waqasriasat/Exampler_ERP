namespace Exampler_ERP.Models.Temp
{
  public class Contract
  {
  }
  public class ContractRenewalViewModel
  {
    public int ContractID { get; set; }
    public int EmployeeID { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int Month { get; set; } // Added field
    public int Year { get; set; }  // Added field
    public DateTime NewStartDate { get; set; } // Added field
    public DateTime NewEndDate { get; set; }   // Added field
  }
}

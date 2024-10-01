using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exampler_ERP.Models.Temp
{
  public class VacationSettleViewModel
  {
    public int VacationID { get; set; }
    [ForeignKey("VacationID")]
    public virtual HR_Vacation? Vacation { get; set; }
    public int EmployeeID { get; set; }
    [ForeignKey("EmployeeID")]
    public virtual HR_Employee? Employee { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalDays { get; set; }
    public int SettleDays { get; set; }
    public int SettleAmount { get; set; }
  }
}

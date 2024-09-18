namespace Exampler_ERP.Models
{
  public class GlobalSetting
  {
    public float? OvertimeRate { get; set; } //1='Full' 1.5='Full and Half' 2='Double'
    public float? OvertimeSalaryProcedure { get; set; } //1='basicSalary' and 2='FullSalary'
  }
}

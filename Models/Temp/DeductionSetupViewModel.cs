using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models.Temp
{
  public class DeductionSetupViewModel
  {
    public int DeductionTypeID { get; set; }
    public int ClassID { get; set; }

    [Required(ErrorMessage = "Please select at least one role.")]
    public List<SalaryTypeDeductionViewModel> SalaryTypeDeductions { get; set; } = new List<SalaryTypeDeductionViewModel>();
  }

  public class SalaryTypeDeductionViewModel
  {
    public int SalaryTypeID { get; set; }
    public string SalaryTypeName { get; set; }
    public bool IsSelected { get; set; }
    public int? DeductionValueID { get; set; }
  }
  public class DeductionSetupIndexViewModel
  {
    public List<Settings_DeductionType> DeductionTypes { get; set; }
    public List<HR_DeductionSetup> DeductionSetups { get; set; }
  }
  public class DeductionSetupListViewModel
  {
    public List<DeductionTypeWithRowCountViewModel> DeductionTypeWithRowCount { get; set; }
  }

  public class DeductionTypeWithRowCountViewModel
  {
    public int DeductionTypeID { get; set; }
    public string? DeductionTypeName { get; set; }
    public int? Class { get; set; }
    public int RowCount { get; set; }
  }
}

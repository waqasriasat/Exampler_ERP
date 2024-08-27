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
}

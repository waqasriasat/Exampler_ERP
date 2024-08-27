namespace Exampler_ERP.Models.Temp
{
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

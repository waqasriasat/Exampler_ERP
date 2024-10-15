using System.ComponentModel.DataAnnotations.Schema;

namespace Exampler_ERP.Models.Temp
{
  public class EmployeeRequestTypeApprovalSetupViewModel
  {
    public int EmployeeRequestTypeID { get; set; }
    public string? EmployeeRequestTypeName { get; set; }
    public int TotalRank { get; set; }

    [ForeignKey("EmployeeRequestTypeID")]
    public virtual Settings_EmployeeRequestType? EmployeeRequestType { get; set; }
  }
  public class EmployeeRequestTypeApprovalSetupIndexViewModel
  {
    public List<Settings_EmployeeRequestType> EmployeeRequestTypes { get; set; }
    public List<HR_EmployeeRequestTypeApprovalSetup> EmployeeRequestTypeApprovalSetups { get; set; }
  }
  public class EmployeeRequestTypeApprovalSetupListViewModel
  {
    public List<EmployeeRequestTypeWithRankCountViewModel> EmployeeRequestTypesWithRankCount { get; set; }
  }

  public class EmployeeRequestTypeWithRankCountViewModel
  {
    public int EmployeeRequestTypeID { get; set; }
    public string? EmployeeRequestTypeName { get; set; }
    public int RankCount { get; set; }

  }
}

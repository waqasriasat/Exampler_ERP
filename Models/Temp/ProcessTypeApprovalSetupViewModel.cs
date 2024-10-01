using System.ComponentModel.DataAnnotations.Schema;

namespace Exampler_ERP.Models.Temp
{
  public class ProcessTypeApprovalSetupViewModel
  {
    public int ProcessTypeID { get; set; }
    public string? ProcessTypeName { get; set; }
    public int TotalRank { get; set; }

    [ForeignKey("ProcessTypeID")]
    public virtual Settings_ProcessType? ProcessType { get; set; }
  }
  public class ProcessTypeApprovalSetupIndexViewModel
  {
    public List<Settings_ProcessType> ProcessTypes { get; set; }
    public List<CR_ProcessTypeApprovalSetup> ProcessTypeApprovalSetups { get; set; }
  }
  public class ProcessTypeApprovalSetupListViewModel
  {
    public List<ProcessTypeWithRankCountViewModel> ProcessTypesWithRankCount { get; set; }
  }

  public class ProcessTypeWithRankCountViewModel
  {
    public int ProcessTypeID { get; set; }
    public string? ProcessTypeName { get; set; }
    public int RankCount { get; set; }
  }
}

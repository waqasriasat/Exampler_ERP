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
}

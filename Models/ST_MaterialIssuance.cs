using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class ST_MaterialIssuance
  {
    [Key]
    public int IssuanceID { get; set; }
    public DateTime IssuanceDate { get; set; }
    public int RequisitionID { get; set; }
    [ForeignKey("RequisitionID")]
    public virtual ST_MaterialRequisition? ST_MaterialRequisitions { get; private set; }
    public int DepartmentTypeID { get; set; }
    public string? Remarks { get; set; }
    public int? FinalApprovalID { get; set; } //didn't use in registration
    public int? ProcessTypeApprovalID { get; set; } //didn't use in registration
    public int? IssuanceStatusTypeID { get; set; }
    [ForeignKey("IssuanceStatusTypeID")]
    public virtual Settings_IssuanceStatusType? IssuanceStatusTypes { get; private set; }

    public virtual List<ST_MaterialIssuanceDetail> MaterialIssuanceDetails { get; set; } = new List<ST_MaterialIssuanceDetail>();
  }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exampler_ERP.Models
{
  public class ST_MaterialRequisition
  {
    [Key]
    public int RequisitionID { get; set; }
    public DateTime RequisitionDate { get; set; }
    public int DepartmentTypeID { get; set; }
    public int? RequisitionStatusTypeID { get; set; }
    [ForeignKey("RequisitionStatusTypeID")]
    public virtual Settings_RequisitionStatusType? RequisitionStatusTypes { get; private set; }
    public string? Remarks { get; set; }
    public int? FinalApprovalID { get; set; } //didn't use in registration
    public int? ProcessTypeApprovalID { get; set; } //didn't use in registration

    public virtual List<ST_MaterialRequisitionDetail> MaterialRequisitionDetails { get; set; } = new List<ST_MaterialRequisitionDetail>();
  }
}

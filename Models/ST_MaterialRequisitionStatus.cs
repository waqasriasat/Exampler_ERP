using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class ST_MaterialRequisitionStatus
  {
    [Key]
    public int RequisitionStatusID { get; set; }
    public int RequisitionID { get; set; }
    public int ActionID { get; set; }
    public DateTime ActionDate { get; set; }
    public int? ActionStatusTypeID { get; set; }
  }
}

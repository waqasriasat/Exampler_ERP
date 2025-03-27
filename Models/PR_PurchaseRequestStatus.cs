using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class PR_PurchaseRequestStatus
  {
    [Key]
    public int PurchaseRequestStatusID { get; set; }
    public int PurchaseRequestID { get; set; }
    public int ActionID { get; set; }
    public DateTime ActionDate { get; set; }
    public int? ActionStatusTypeID { get; set; }
  }
}

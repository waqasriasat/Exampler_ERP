using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exampler_ERP.Models
{
  public class FI_ChequeBookDetail
  {
    [Key]
    public int ChequeBookDetailID { get; set; }
    public int ChequeBookID { get; set; }
    [ForeignKey("ChequeBookID")]
    public virtual FI_ChequeBook? ChequeBook { get; set; }
    public int ChequeNo { get; set; }
    public int? GLID { get; set; }
    public string? Status { get; set; } = "Unbind"; // Default value
  }
}

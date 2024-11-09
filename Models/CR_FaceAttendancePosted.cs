using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class CR_FaceAttendancePosted
  {
    [Key]
    public int FaceAttendancePostedeID { get; set; }
    public int? BranchTypeID { get; set; }
    [ForeignKey("BranchTypeID")]
    public virtual Settings_BranchType? BranchType { get; set; }
    public int? MonthTypeID { get; set; }
    [ForeignKey("MonthTypeID")]
    public virtual Settings_MonthType? MonthType { get; set; }
    public int? Year { get; set; }
    public int? PostedID { get; set; }
  }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exampler_ERP.Models
{
  public class CR_AccessRightsByUser
  {
    [Key]
    public int AccessRightID { get; set; } // Unique identifier for access rights
    public int ActionSOR { get; set; } // Sequence or order of action
    public int ActionType { get; set; } // 1 = Page, 2 = Button
    public int ModuleID { get; set; }
    public int MenuID { get; set; }
    public string? ActionName { get; set; } // Name of the action (page or button)
    public int UserID { get; set; }
    [ForeignKey("UserID")]
    public virtual CR_User? user { get; set; }
  }
}

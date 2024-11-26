using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class CR_AccessRightsByRole
  {
    [Key]
    public int AccessRightID { get; set; } // Unique identifier for access rights
    public int ActionSOR { get; set; } // Sequence or order of action
    public int ActionType { get; set; } // 1 = Page, 2 = Button
    public int ModuleID { get; set; }
    public int MenuID { get; set; }
    public string? ActionName { get; set; } // Name of the action (page or button)

    // Columns from 1 to 50
    public int? _1 { get; set; }
    public int? _2 { get; set; }
    public int? _3 { get; set; }
    public int? _4 { get; set; }
    public int? _5 { get; set; }
    public int? _6 { get; set; }
    public int? _7 { get; set; }
    public int? _8 { get; set; }
    public int? _9 { get; set; }
    public int? _10 { get; set; }
    public int? _11 { get; set; }
    public int? _12 { get; set; }
    public int? _13 { get; set; }
    public int? _14 { get; set; }
    public int? _15 { get; set; }
    public int? _16 { get; set; }
    public int? _17 { get; set; }
    public int? _18 { get; set; }
    public int? _19 { get; set; }
    public int? _20 { get; set; }
    public int? _21 { get; set; }
    public int? _22 { get; set; }
    public int? _23 { get; set; }
    public int? _24 { get; set; }
    public int? _25 { get; set; }
    public int? _26 { get; set; }
    public int? _27 { get; set; }
    public int? _28 { get; set; }
    public int? _29 { get; set; }
    public int? _30 { get; set; }
    public int? _31 { get; set; }
    public int? _32 { get; set; }
    public int? _33 { get; set; }
    public int? _34 { get; set; }
    public int? _35 { get; set; }
    public int? _36 { get; set; }
    public int? _37 { get; set; }
    public int? _38 { get; set; }
    public int? _39 { get; set; }
    public int? _40 { get; set; }
    public int? _41 { get; set; }
    public int? _42 { get; set; }
    public int? _43 { get; set; }
    public int? _44 { get; set; }
    public int? _45 { get; set; }
    public int? _46 { get; set; }
    public int? _47 { get; set; }
    public int? _48 { get; set; }
    public int? _49 { get; set; }
    public int? _50 { get; set; }
  }


}

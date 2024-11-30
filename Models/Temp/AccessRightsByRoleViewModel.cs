using System.ComponentModel.DataAnnotations.Schema;

namespace Exampler_ERP.Models.Temp
{
  public class AccessRightsByRoleViewModel
  {
    public int RoleID { get; set; }
    public string? RoleName { get; set; }
    public int TotalAccessCount { get; set; }

    [ForeignKey("AccessRightsID")]
    public virtual Settings_RoleType? RoleType { get; set; }
  }
  public class AccessRightsByRoleIndexViewModel
  {
    public List<Settings_RoleType> RoleTypes { get; set; }
    public List<CR_AccessRightsByRole> AccessRightsByRoles { get; set; }
  }
  public class AccessRightsByRoleListViewModel
  {
    public List<AccessRightsWithRoleCountViewModel> AccessRightsWithRoleCount { get; set; }
  }

  public class AccessRightsWithRoleCountViewModel
  {
    public int RoleID { get; set; }
    public string? RoleName { get; set; }
    public int TotalAccessCount { get; set; }
  }
}

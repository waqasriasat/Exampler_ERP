using System.ComponentModel.DataAnnotations.Schema;

namespace Exampler_ERP.Models.Temp
{
  public class AccessRightsByUserViewModel
  {
    public int UserID { get; set; }
    public string? UserName { get; set; }
    public int TotalAccessCount { get; set; }

    [ForeignKey("UserID")]
    public virtual CR_User? user { get; set; }
  }
  public class AccessRightsByUserIndexViewModel
  {
    public List<CR_User> users { get; set; }
    public List<CR_AccessRightsByUser> AccessRightsByUsers { get; set; }
  }
  public class AccessRightsByUserListViewModel
  {
    public List<AccessRightsWithUserCountViewModel> AccessRightsWithUserCount { get; set; }
  }

  public class AccessRightsWithUserCountViewModel
  {
    public int UserID { get; set; }
    public string? UserName { get; set; }
    public int TotalAccessCount { get; set; }
  }
}

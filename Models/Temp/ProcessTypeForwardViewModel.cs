using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models.Temp
{
  public class ProcessTypeForwardViewModel
  {
    public int ProcessTypeID { get; set; }

    [Required(ErrorMessage = "Please select at least one role.")]
    public List<int> SelectedRoleIds { get; set; } = new List<int>();
  }
  public class ProcessTypeForwardIndexViewModel
  {
    public List<Settings_ProcessType> ProcessTypes { get; set; }
    public List<CR_ProcessTypeForward> ProcessTypeForwards { get; set; }
  }
  public class ProcessTypeForwardListViewModel
  {
    public List<ProcessTypeWithRoleCountViewModel> ProcessTypesWithRoleCount { get; set; }
  }

  public class ProcessTypeWithRoleCountViewModel
  {
    public int ProcessTypeID { get; set; }
    public string? ProcessTypeName { get; set; }
    public int RoleCount { get; set; }
  }
}

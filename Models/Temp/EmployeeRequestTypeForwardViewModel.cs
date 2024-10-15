using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models.Temp
{
  public class EmployeeRequestTypeForwardViewModel
  {
    public int EmployeeRequestTypeID { get; set; }

    [Required(ErrorMessage = "Please select at least one role.")]
    public List<int> SelectedRoleIds { get; set; } = new List<int>();
  }
  public class EmployeeRequestTypeForwardIndexViewModel
  {
    public List<Settings_EmployeeRequestType> EmployeeRequestTypes { get; set; }
    public List<HR_EmployeeRequestTypeForward> EmployeeRequestTypeForwards { get; set; }
  }
  public class EmployeeRequestTypeForwardListViewModel
  {
    public List<EmployeeRequestTypeWithRoleCountViewModel> EmployeeRequestTypesWithRoleCount { get; set; }
  }

  public class EmployeeRequestTypeWithRoleCountViewModel
  {
    public int EmployeeRequestTypeID { get; set; }
    public string? EmployeeRequestTypeName { get; set; }
    public int RoleCount { get; set; }
  }
}

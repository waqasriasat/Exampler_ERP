using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models.Temp
{
  public class ProcessTypeForwardViewModel
  {
    public int ProcessTypeID { get; set; }

    [Required(ErrorMessage = "Please select at least one role.")]
    public List<int> SelectedRoleIds { get; set; } = new List<int>();
  }
}

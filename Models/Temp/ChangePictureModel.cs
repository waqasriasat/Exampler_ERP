using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models.Temp
{
  public class ChangePictureModel
  {
    [Key]
    public int EmployeeID { get; set; }
    public byte[]? Picture { get; set; }
  }
}

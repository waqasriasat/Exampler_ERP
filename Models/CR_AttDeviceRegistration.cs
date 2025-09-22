using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class CR_AttDeviceRegistration
  {
    [Key]
    public int ID { get; set; }
    public int ActiveYNID { get; set; }
    public string SerialNumber { get; set; }
    public string IP { get; set; }
    public string Mac { get; set; }
    public string Location { get; set; }

  }
}

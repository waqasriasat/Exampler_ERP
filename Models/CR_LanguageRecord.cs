using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class CR_LanguageRecord
  {
    [Key]
    public int LanguageRecordId { get; set; }
    public string LabelName { get; set; }
    public string LabelValue { get; set; }
    public string Culture { get; set; }
  }
}

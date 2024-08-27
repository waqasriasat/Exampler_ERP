using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class Settings_Branch
  {
    [Key]
    public int BranchID { get; set; }
    public string? BranchName { get; set; }
    public int? DeleteYNID { get; set; }
    public int? ActiveYNID { get; set; }
    public string? POBox { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }
    public string? Street { get; set; }
    public string? Phone { get; set; }
    public string? Fax { get; set; }
    public string? Mobile { get; set; }
    public string? Address { get; set; }
    public DateTime? Date { get; set; }
  }
}

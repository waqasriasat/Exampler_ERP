using Exampler_ERP.Models.Temp;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exampler_ERP.Models
{
  public class HR_DocumentUpload
  {
    [Key]
    public int DocumentUploadID { get; set; }  // Primary key

    public int EmployeeID { get; set; }  // Foreign key for Employee
    [ForeignKey("EmployeeID")]
    public virtual HR_Employee? Employee { get; set; }

    public string DocumentName { get; set; }  // Original name of the document
   
    public string DocumentType { get; set; }  // File type, e.g., "pdf", "docx"
    
    public byte[]? DocumentData { get; set; }  // Binary data of the document

    public DateTime Date { get; set; }  // Date and time of upload

    [MaxLength(1000)]
    public string? Description { get; set; }  // Optional description of the document

    public int DeleteYNID { get; set; }  // Status: 1 = Deleted, 0 = Not Deleted
    
  }
}

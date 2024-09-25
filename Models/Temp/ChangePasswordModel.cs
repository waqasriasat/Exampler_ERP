using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models.Temp
{
  public class ChangePasswordModel
  {
    [Required]
    public int EmployeeID { get; set; } // Employee unique ID

    [Required]
    public string UserName { get; set; } // Employee's Username

    [Required(ErrorMessage = "Old password is required.")]
    [DataType(DataType.Password)]
    public string OldPassword { get; set; } // Employee's old password

    [Required(ErrorMessage = "New password is required.")]
    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "Password should be at least 6 characters long.")]
    public string NewPassword { get; set; } // Employee's new password

    [Required(ErrorMessage = "Confirm new password is required.")]
    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "New Password and Confirm Password do not match.")]
    public string ConfirmPassword { get; set; } // Confirm new password
  }
}

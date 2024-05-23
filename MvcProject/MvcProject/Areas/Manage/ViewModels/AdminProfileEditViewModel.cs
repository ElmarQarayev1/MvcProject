using System;
using System.ComponentModel.DataAnnotations;

namespace MvcProject.Areas.Manage.ViewModels
{
    public class AdminProfileEditViewModel
    {
          
        [Required]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

}


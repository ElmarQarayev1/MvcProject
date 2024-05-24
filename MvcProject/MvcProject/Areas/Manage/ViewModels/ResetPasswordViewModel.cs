using System;
using System.ComponentModel.DataAnnotations;

namespace MvcProject.Areas.Manage.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        [MinLength(8)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }


        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword")]       
        public string ConfirmPassword { get; set; }
    }

}


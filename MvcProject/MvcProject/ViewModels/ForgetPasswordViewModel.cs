using System;
using System.ComponentModel.DataAnnotations;

namespace MvcProject.ViewModels
{
    public class ForgetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [MaxLength(100)]
        [MinLength(3)]
        public string Email { get; set; }
    }
}


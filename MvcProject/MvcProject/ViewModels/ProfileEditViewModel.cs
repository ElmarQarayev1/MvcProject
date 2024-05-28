using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MvcProject.Models;

namespace MvcProject.ViewModels
{
    public class ProfileEditViewModel
    {
        [MaxLength(25)]
        [MinLength(5)]
        [Required]
        public string UserName { get; set; }

        [MaxLength(25)]
        [MinLength(5)]
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [MaxLength(25)]
        [MinLength(5)]
        [Required]
        public string FullName { get; set; }

        [MaxLength(25)]
        [MinLength(8)]
        [DataType(DataType.Password)]
        public string? CurrentPassword { get; set; }

        [MaxLength(25)]
        [MinLength(8)]
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }

        [MaxLength(25)]
        [MinLength(8)]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword))]
        public string? ConfirmNewPassword { get; set; }

        public List<Application> Applications { get; set; } = new List<Application>();

        public bool HasPassword { get; set; }
        public bool IsGoogleUser { get; set; }
    }
}

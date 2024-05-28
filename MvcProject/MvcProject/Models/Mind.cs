using System;
using System.ComponentModel.DataAnnotations;
using MvcProject.Models.Enum;

namespace MvcProject.Models
{
	public class Mind
	{
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? AppUserId { get; set; }       
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        public string Text { get; set; }
        [Required]
        public string Subject { get; set; }

        public AppUser? AppUser { get; set; }      
    }
}

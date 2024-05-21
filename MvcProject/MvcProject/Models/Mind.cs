using System;
using System.ComponentModel.DataAnnotations;
using MvcProject.Models.Enum;

namespace MvcProject.Models
{
	public class Mind
	{
        public int Id { get; set; }
        public string Name { get; set; }
        public string? AppUserId { get; set; }       
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public MindStatus Status { get; set; } = MindStatus.Pending;
        [EmailAddress]
        public string Email { get; set; }
        public string Text { get; set; }
        public string Subject { get; set; }
        public AppUser? AppUser { get; set; }      
    }
}

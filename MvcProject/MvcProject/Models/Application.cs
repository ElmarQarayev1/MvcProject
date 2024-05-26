using System;
using System.ComponentModel.DataAnnotations;
using MvcProject.Models.Enum;

namespace MvcProject.Models
{
	public class Application:BaseEntity
	{
        public string? AppUserId { get; set; }
        public int CourseId { get; set; }

        [MaxLength(50)]
        [MinLength(3)]
        public string? FullName { get; set; }
        [MaxLength(100)]

        [EmailAddress]
        public string? Email { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedAt { get; set; }

        public ApplicationStatus Status { get; set; }

        public AppUser? AppUser { get; set; }
        public Course? Course { get; set; }

    }
}


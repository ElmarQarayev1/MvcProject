using System;
using MvcProject.Attributes.ValidationAttributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcProject.Models
{
	public class Course:AuditEntity
	{
		public string? Img { get; set; }

		public string Name { get; set; }

		public string Desc { get; set; }

		public string? Certification { get; set; }

		public DateTime StartDate { get; set; }

		public int Duration { get; set; }

		public int ClassDuration { get; set; }

		public string? Language { get; set; }

		public int StudentsCount { get; set; }

		public double Price { get; set; }

		public string? HowtoApply { get; set; }

		public int CategoryId { get; set; }

		public Category? Category { get; set; }

		public List<CourseTag>? CourseTags { get; set; } = new List<CourseTag>();
        [NotMapped]
        public List<int>? TagIds { get; set; } = new List<int>();
        [NotMapped]
        [MaxSize(1024 * 1024 * 2)]
        [AllowedFileTypes("image/png", "image/jpeg")]
        public IFormFile? ImageFile { get; set; }

    }
}


using System;
namespace MvcProject.Models
{
	public class Course:BaseEntity
	{
		public string Name { get; set; }

		public string Desc { get; set; }

		public string Certification { get; set; }

		public DateTime StartDate { get; set; }

		public int Duration { get; set; }

		public int ClassDuration { get; set; }

		public string Language { get; set; }

		public string StudentsCount { get; set; }

		public double Price { get; set; }

		public string HowtoApply { get; set; }

		public int CategoryId { get; set; }

		public Category Category { get; set; }

		public List<CourseTag> CourseTags { get; set; }

	}
}


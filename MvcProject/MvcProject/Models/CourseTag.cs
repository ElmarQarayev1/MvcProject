using System;
namespace MvcProject.Models
{
	public class CourseTag:BaseEntity
	{
		public int TagId { get; set; }

		public int CourseId { get; set; }

		public Course Course { get; set; }

		public Tag Tag { get; set; }
	}
}


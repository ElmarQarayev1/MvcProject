using System;
namespace MvcProject.Models
{
	public class Tag:BaseEntity
	{
		public string Name { get; set; }

		public List<EventTag>? EventTags { get; set; }

		public List<CourseTag>? CourseTags { get; set; }

	}
}


using System;
namespace MvcProject.Models
{
	public class Category:BaseEntity
	{

		public string Name { get; set; }

		public List<Course> Courses { get; set; }

		public List<Event> Events { get; set; }
	}
}


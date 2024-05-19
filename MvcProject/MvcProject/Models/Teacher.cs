using System;
namespace MvcProject.Models
{
	public class Teacher:BaseEntity
	{
		public string Img { get; set; }

		public string FullName { get; set; }

		public string Position { get; set; }

		public List<SocialMedia> SocialMedias { get; set; }

		public string Desc { get; set; }

		public List<EventTeacher> EventTeachers { get; set; }

		
	}
}


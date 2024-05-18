using System;
namespace MvcProject.Models
{
	public class Teacher:BaseEntity
	{
		public string FullName { get; set; }

		public string Position { get; set; }

		public List<SocialMedia> SocialMedias { get; set; }

		public string Desc { get; set; }

		public string Degree { get; set; }

		public string Experience { get; set; }

		public string Hobbies { get; set; }

		public  string Faculty { get; set; }

		public string Email { get; set; }

		public string Phone { get; set; }

		public string Skype { get; set; }

		public int LanguagePercent { get; set; }

		public int DesignPercent { get; set; }

		public int TeamLeaderPercent { get; set; }

		public int DevelopmentPercent { get; set; }

		public	int InnovationPercent { get; set; }

		public int CommunacationPercent { get; set; }

		public List<EventTeacher> EventTeachers { get; set; }

		

	}
}


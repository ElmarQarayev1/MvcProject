using System;
using MvcProject.Attributes.ValidationAttributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcProject.Models
{
	public class Teacher:BaseEntity
	{
		public string? Img { get; set; }

		public string FullName { get; set; }

		public string Position { get; set; }

		public string InstagramUrl { get; set; }

		public string LinkEdinUrl { get; set; }

		public string PinterestUrl { get; set; }

		public string VingUrl { get; set; }

		public string Desc { get; set; }

        public string Degree { get; set; }

        public string Experience { get; set; }

        public int LanguagePercent { get; set; }

        public int DesignPercent { get; set; }

        public int TeamLeaderPercent { get; set; }

        public int DevelopmentPercent { get; set; }

        public int InnovationPercent { get; set; }

        public int CommunacationPercent { get; set; }

        public string Faculty { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Skype { get; set; }

        public List<EventTeacher>? EventTeachers { get; set; } = new List<EventTeacher>();
        [NotMapped]
        [MaxSize(1024 * 1024 * 2)]
        [AllowedFileTypes("image/png", "image/jpeg")]
        public IFormFile? ImageFile { get; set; }
        [NotMapped]
        public List<int>? EventIds { get; set; } = new List<int>();

    }
}


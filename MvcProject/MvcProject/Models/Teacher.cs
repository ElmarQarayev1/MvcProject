using System;
using MvcProject.Attributes.ValidationAttributes;
using System.ComponentModel.DataAnnotations.Schema;

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
        [NotMapped]
        [MaxSize(1024 * 1024 * 2)]
        [AllowedFileTypes("image/png", "image/jpeg")]
        public IFormFile? ImageFile { get; set; }

    }
}


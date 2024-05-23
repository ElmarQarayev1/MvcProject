using System;
using MvcProject.Attributes.ValidationAttributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcProject.Models
{
	public class TestiMony:BaseEntity
	{
		public string? ImgName { get; set; }

		public string FullName { get; set; }

        public string Desc { get; set; }

		public string Position { get; set; }
        [NotMapped]
        [MaxSize(1024 * 1024 * 2)]
        [AllowedFileTypes("image/png", "image/jpeg")]
        public IFormFile? ImageFile { get; set; }
        public int Order { get; set; }

    }
}


using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MvcProject.Attributes.ValidationAttributes;

namespace MvcProject.Models
{
	public class Slider:BaseEntity
	{
        [MaxLength(100)]
        public string? Title1 { get; set; }
        [MaxLength(100)]
        public string? Title2 { get; set; }
        [MaxLength(250)]
        public string? Desc { get; set; }
        [MaxLength(100)]
        public string? BtnText { get; set; }
        [MaxLength(250)]
        public string? BtnUrl { get; set; }
        public string? ImageName { get; set; }
        [NotMapped]
        [MaxSize(1024 * 1024 * 2)]
        [AllowedFileTypes("image/png", "image/jpeg")]
        public IFormFile? ImageFile { get; set; }
        public int Order { get; set; }
    }
}


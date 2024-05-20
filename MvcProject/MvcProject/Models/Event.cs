﻿using System;
using System.ComponentModel.DataAnnotations.Schema;
using MvcProject.Attributes.ValidationAttributes;

namespace MvcProject.Models
{
	public class Event:BaseEntity
	{
        public string? Img { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime  EndTime { get; set; }

        public string? Name { get; set; }

        public string? Venue { get; set; }

        public int? CategoryId { get; set; }

        public Category? Category { get; set; }

        public string? Desc { get; set; }

        public List<EventTag>? EventTags { get; set; } = new List<EventTag>();

        public List<EventTeacher>? EventTeachers { get; set; } = new List<EventTeacher>();
        [NotMapped]
        public List<int>? TagIds { get; set; } = new List<int>();
        [NotMapped]
        [MaxSize(1024 * 1024 * 2)]
        [AllowedFileTypes("image/png", "image/jpeg")]
        public IFormFile? ImageFile { get; set; }
    }
}

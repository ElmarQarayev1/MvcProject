using System;
using MvcProject.Models;

namespace MvcProject.ViewModels
{
	public class CourseDetailViewModel
	{
        public Application Application { get; set; }
        public Course Course { get; set; }
        public List<Category> Categories { get; set; }
        public List<Tag> Tags { get; set; }
    }
}

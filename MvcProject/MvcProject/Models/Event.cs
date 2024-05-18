using System;
namespace MvcProject.Models
{
	public class Event:BaseEntity
	{
        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string Name { get; set; }

        public string Venue { get; set; }

        public int CategoryId { get; set; }

        public Category Category { get; set; }

        public string Desc { get; set; }

        public List<EventTag> EventTags { get; set; }
        public List<EventTeacher> EventTeachers { get; set; }
        

    }
}

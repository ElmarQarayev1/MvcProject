using System;
namespace MvcProject.Models
{
	public class EventTeacher:BaseEntity
	{
		public int EventId { get; set; }

		public int TeacherId { get; set; }

		public Event? Event { get; set; }

		public Teacher? Teacher { get; set; }

	}
}


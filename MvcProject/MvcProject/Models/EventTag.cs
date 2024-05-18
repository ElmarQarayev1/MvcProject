using System;
namespace MvcProject.Models
{
	public class EventTag:BaseEntity
	{
		public int EventId { get; set; }
		public int TagId { get; set; }
		public Tag Tag { get; set; }
		public Event Event { get; set; } 		
	}
}


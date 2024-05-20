using System;
using MvcProject.Models;

namespace MvcProject.ViewModels
{
	public class EventDetailViewModel
	{
		public Event Event { get;set; }

		public List<Category> Categories { get; set; }

		
	}
}


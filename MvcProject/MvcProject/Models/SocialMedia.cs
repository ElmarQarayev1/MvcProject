using System;
using Microsoft.AspNetCore.Mvc;

namespace MvcProject.Models
{
	public class SocialMedia:BaseEntity
	{
		public string Media { get; set; }
		public int TeacherId { get; set; }
		public Teacher Teacher { get; set; }
	}
}

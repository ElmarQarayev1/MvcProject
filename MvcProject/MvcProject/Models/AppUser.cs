using System;
using Microsoft.AspNetCore.Identity;

namespace MvcProject.Models
{
	public class AppUser:IdentityUser
	{
		public string FullName { get; set; }
	}
}


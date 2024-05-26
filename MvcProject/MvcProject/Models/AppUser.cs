using System;
using Microsoft.AspNetCore.Identity;

namespace MvcProject.Models
{
	public class AppUser:IdentityUser
	{
		public string FullName { get; set; }

        public string? ConnectionId { get; set; }

        public DateTime? LastConnectedAt { get; set; }

        public bool IsPasswordResetRequired { get; set; } = true;
    }
}



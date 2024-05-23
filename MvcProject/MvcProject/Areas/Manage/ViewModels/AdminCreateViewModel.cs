using System;
using System.ComponentModel.DataAnnotations;

namespace MvcProject.Areas.Manage.ViewModels
{
	public class AdminCreateViewModel
	{
		[Required]
		[MaxLength(50)]
		[MinLength(3)]
		public string UserName { get; set; }

		[Required]
		[MaxLength(40)]
		[MinLength(8)]
		public string Password { get; set; }
	}
}


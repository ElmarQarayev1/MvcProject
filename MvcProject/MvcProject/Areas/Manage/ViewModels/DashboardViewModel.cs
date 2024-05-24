using System;
using Microsoft.EntityFrameworkCore;
using MvcProject.Models.Enum;

namespace MvcProject.Areas.Manage.ViewModels
{
	public class DashboardViewModel
	{
		public double MonthlyBenefit { get; set; }

		public double YearlyBenefit { get; set; }

		public  double AcceptanceRate { get; set; }

        public int PendingApplications { get; set; }

		

    }
}


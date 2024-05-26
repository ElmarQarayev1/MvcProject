
using System;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcProject.Areas.Manage.ViewModels;
using MvcProject.Data;
using MvcProject.Models.Enum;

namespace MvcProject.Areas.Manage.Controllers
{
	[Area("manage")]
	[Authorize(Roles ="admin,superadmin")]
	public class DashboardController:Controller
	{
        public readonly AppDbContext _context;
        public DashboardController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
           
            DateTime firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);   
            double monthlyEarning = _context.Applications
                .Where(a => a.Status == ApplicationStatus.Accepted && a.CreatedAt >= firstDayOfMonth)
                .Sum(a => a.Course.Price);

            DateTime firstDayOfYear = new DateTime(DateTime.Now.Year, 1, 1);

            double yearlyEarnings = _context.Applications
                .Where(a => a.Status == ApplicationStatus.Accepted && a.CreatedAt >= firstDayOfYear)
                .Sum(a => a.Course.Price);

            int totalApplications = _context.Applications.Count();  
            int acceptedApplications = _context.Applications.Count(a => a.Status == ApplicationStatus.Accepted);
            double acceptanceRate = (double)acceptedApplications / totalApplications * 100;


            int pendingApplications = _context.Applications.Count(a => a.Status == ApplicationStatus.Pending);

            int pendingCount = _context.Applications.Count(a => a.Status == ApplicationStatus.Pending);
            int acceptedCount = _context.Applications.Count(a => a.Status == ApplicationStatus.Accepted);
            int rejectedCount = _context.Applications.Count(a => a.Status == ApplicationStatus.Rejected);

            ViewBag.PendingCount = pendingCount;
            ViewBag.AcceptedCount = acceptedCount;
            ViewBag.RejectedCount = rejectedCount;

            DashboardViewModel dashboardViewModel = new DashboardViewModel()
            {
                MonthlyBenefit = monthlyEarning,
                YearlyBenefit = yearlyEarnings,
                AcceptanceRate = acceptanceRate,
                PendingApplications=pendingApplications,               
            };        
            return View(dashboardViewModel);
        }

        [HttpGet]
        public IActionResult GetMonthlyEarnings()
        {
            DateTime firstDayOfYear = new DateTime(DateTime.Now.Year, 1, 1);
            var monthlyEarnings = new List<double>();
            var months = new List<string>();

            for (int i = 1; i <= 12; i++)
            {
                DateTime firstDayOfMonth = new DateTime(DateTime.Now.Year, i, 1);
                DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
                double monthlyEarning = _context.Applications
                    .Where(a => a.Status == ApplicationStatus.Accepted
                            && a.CreatedAt >= firstDayOfMonth
                            && a.CreatedAt <= lastDayOfMonth)
                    .Select(a => a.Course.Price)
                    .Sum();
                monthlyEarnings.Add(monthlyEarning);
                months.Add(firstDayOfMonth.ToString("MMM"));
            }

            var earningsData = new { months, earnings = monthlyEarnings };

            return Json(earningsData);
        }

    }
}

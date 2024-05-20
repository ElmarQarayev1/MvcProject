using System;
using Microsoft.EntityFrameworkCore;
using MvcProject.Data;

namespace MvcProject.Services
{
	public class LayoutService
	{
        private readonly AppDbContext _context;

        public LayoutService(AppDbContext context)
        {
            _context = context;

        }
        public Dictionary<String, String> GetSettings()
        {
            return _context.Settings.ToDictionary(x => x.Key, x => x.Value);
        }
    }
}


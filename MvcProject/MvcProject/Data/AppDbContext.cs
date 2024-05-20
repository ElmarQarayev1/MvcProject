using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MvcProject.Models;

namespace MvcProject.Data
{
	public class AppDbContext: IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<Slider> Sliders { get; set; }

        public DbSet<Feature> Features { get; set; }

        public DbSet<Info> Infos { get; set; }

        public DbSet<AppUser> AppUsers { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Course> Courses { get; set; }

        public DbSet<CourseTag> CourseTags { get; set; }

        public DbSet<Event> Events { get; set; }

        public DbSet<EventTag> EventTags { get; set; }

        public DbSet<EventTeacher> EventTeachers { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<Teacher> Teachers { get; set; }

        public DbSet<Setting> Settings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }


    }
}


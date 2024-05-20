using System;
using MvcProject.Models;

namespace MvcProject.ViewModels
{
    public class EventIndexViewModel
    {
        public List<Event> Events { get; set; }
        public List<Category> Categories { get; set; }
    }
}


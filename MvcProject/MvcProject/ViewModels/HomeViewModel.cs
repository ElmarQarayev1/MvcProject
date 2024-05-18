using System;
using MvcProject.Models;

namespace MvcProject.ViewModels
{
	public class HomeViewModel
	{
		
	    public List<Slider> Sliders { get; set; }
		public List<Feature> Features { get; set; }
		public List<Info> Infos { get; set; }

    }
}


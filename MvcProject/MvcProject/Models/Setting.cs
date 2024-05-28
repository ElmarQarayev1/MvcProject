using System;
using System.ComponentModel.DataAnnotations;
using MvcProject.Attributes.ValidationAttributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcProject.Models
{
	public class Setting
	{
        
            [Key]
            public string Key { get; set; }
            [MaxLength(500)]
            public string Value { get; set; }         

       
    }
}



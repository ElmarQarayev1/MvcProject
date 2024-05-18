using System;
using System.ComponentModel.DataAnnotations;

namespace MvcProject.Attributes.ValidationAttributes
{
	public class MaxSizeAttribute:ValidationAttribute
	{
        private int _byteSize;
        public MaxSizeAttribute(int byteSize)
        {
            _byteSize = byteSize;
        }
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {

            List<IFormFile> list = new List<IFormFile>();

            if (value is List<IFormFile> files) list = files;
            else if (value is IFormFile file) list.Add(file);


            foreach (var f in list)
            {
                if ((f != null))
                {
                    if (f.Length > _byteSize)
                    {
                        double mb = _byteSize / 1024d / 1024d;
                        return new ValidationResult($"File must be less or equal than {mb.ToString("0.##")}mb");
                    }
                }
            }

            return ValidationResult.Success;

        }
    }
}


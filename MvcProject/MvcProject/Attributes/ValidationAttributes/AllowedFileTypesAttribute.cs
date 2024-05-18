using System;
using System.ComponentModel.DataAnnotations;

namespace MvcProject.Attributes.ValidationAttributes
{
	public class AllowedFileTypesAttribute:ValidationAttribute
	{
        private string[] _types;
        public AllowedFileTypesAttribute(params string[] types)
        {
            _types = types;
        }
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            List<IFormFile> list = new List<IFormFile>();

            if (value is List<IFormFile> files) list = files;
            else if (value is IFormFile file) list.Add(file);

            foreach (var f in list)
            {
                if (f != null)
                {
                    if (!_types.Contains(f.ContentType))
                    {
                        string errors = "File must be one of the types: " + String.Join(",", _types);
                        return new ValidationResult(errors);
                    }
                }
            }
            return ValidationResult.Success;
        }

    }
}


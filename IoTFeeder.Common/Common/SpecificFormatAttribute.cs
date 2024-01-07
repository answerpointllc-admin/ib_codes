using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;

namespace IoTFeeder.Common.Common
{
    public class SpecificFormatAttribute : ValidationAttribute
    {
        private readonly string[] _formats;

        public SpecificFormatAttribute(params string[] formats)
        {
            _formats = formats;
        }

        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return true;
            }

            if (value is IFormFile file)
            {
                var extension = '.' + file.FileName.Split('.')[^1];

                return _formats.Any(x => string.Equals(extension, x, StringComparison.OrdinalIgnoreCase));
            }

            if (value is string name)
            {
                var extension = '.' + name.Split('.')[^1];

                return _formats.Any(x => string.Equals(extension, x, StringComparison.OrdinalIgnoreCase));
            }

            return false;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var formats = _formats.Aggregate((x, y) => x + ", " + y);

            return IsValid(value) ? ValidationResult.Success : new ValidationResult($"Only {formats} is valid");
        }
    }
}

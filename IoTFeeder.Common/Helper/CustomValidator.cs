using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace JTC.Common.Helper
{
    public static class CustomValidator<T>
    {
        public static List<ValidationResult> IsValid(T model)
        {
            List<ValidationResult> results = new List<ValidationResult>();
            Validator.TryValidateObject(model, new ValidationContext(model), results, false);

            return results;
        }
    }
}

using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Oceanarium.Models;

namespace Oceanarium.Validations
{
    public class DateIsValid : ValidationAttribute
    {
        public string StartDateProperty { get; }
        public string EndDateProperty { get; }

        public DateIsValid(string startDateProperty, string endDateProperty)
        {
            StartDateProperty = startDateProperty;
            EndDateProperty = endDateProperty;
        }
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            var type = validationContext.ObjectType;

            var startProp = type.GetProperty(StartDateProperty);
            var endProp = type.GetProperty(EndDateProperty);


            if (startProp == null || endProp == null)
            {
                return new ValidationResult("Invalid property names.");
            }


            var startDateValue = startProp.GetValue(validationContext.ObjectInstance) as DateTime?;
            var endDateValue = endProp.GetValue(validationContext.ObjectInstance) as DateTime?;

            if (validationContext.ObjectInstance is Exibition exibition && exibition.IsPermanent)
            {
                return ValidationResult.Success;
            }


            if (startDateValue >= endDateValue)
            {
                return new ValidationResult($"{EndDateProperty} must be after {StartDateProperty}");
            }

            


            return ValidationResult.Success;
        }
        public override string FormatErrorMessage(string name)
        {
            return "Invalid status";
        }

    }
}

using System.ComponentModel.DataAnnotations;

namespace Oceanarium.Validations
{
    public class DiscountIsValid : ValidationAttribute
    {
        private static readonly string[] ValidDiscounts = { "Adult", "Child", "Student", "Senior" };
        public override bool IsValid(object? value)
        {
            var discountType = value as string;
            return discountType != null && ValidDiscounts.Contains(discountType);
        }

        public override string FormatErrorMessage(string name)
        {
            return "Invalid discount type";
        }
    }
}

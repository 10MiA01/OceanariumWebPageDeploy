using System.ComponentModel.DataAnnotations;

namespace Oceanarium.Validations
{
    public class StatusIsValid : ValidationAttribute
    {
        private static readonly string[] ValidStatuses = { "Active", "Cancelled", "Refunded", "Finished" };

        public override bool IsValid(object value)
        {
            var status = value as string;
            return status != null && ValidStatuses.Contains(status);
        }
    }
}

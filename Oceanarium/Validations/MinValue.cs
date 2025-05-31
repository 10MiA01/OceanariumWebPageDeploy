using System.ComponentModel.DataAnnotations;

namespace Oceanarium.Validations
{
    public class MinValue: ValidationAttribute
    {
        private readonly int  _minvalue;

        public MinValue(int minvalue)
        {
            _minvalue = minvalue;
        }

        public override bool IsValid(object? value)
        {
            if (value == null) {return false;}
            if(value is int intValue)
            {
                return intValue >= _minvalue;
            }

            if (value is decimal decimalValue)
            {
                return decimalValue >= _minvalue;
            }

            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} must be greater than or equal to {_minvalue}.";
        }
    }
}

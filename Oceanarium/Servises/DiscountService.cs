using Oceanarium.Servises.Interfaces;

namespace Oceanarium.Servises
{
    public class DiscountService : IDiscountService
    {

        public decimal CalculateDiscountedPrice(decimal originalPrice, string discountType)
        {

            decimal discountedPrice = originalPrice;

            if (discountType == "Child")
            {
                discountedPrice = originalPrice * 0.5m;
            }
            else if (discountType == "Adult")
            {
                discountedPrice = originalPrice;
            }
            else if (discountType == "Student")
            {
                discountedPrice = originalPrice * 0.75m;
            }
            else if (discountType == "Senior")
            {
                discountedPrice = originalPrice * 0.75m;
            }

            return discountedPrice;
        }
    }
}

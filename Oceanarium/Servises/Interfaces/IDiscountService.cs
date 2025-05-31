namespace Oceanarium.Servises.Interfaces
{
    public interface IDiscountService
    {
        decimal CalculateDiscountedPrice(decimal originalPrice, string discountType);
    }
}

using Oceanarium.Models;
using Oceanarium.ViewModels;

namespace Oceanarium.Servises.Interfaces
{
    public interface IFilterOrderService
    {
        Task<List<Order>> GetFilteredAsync(OrderFilterParams filterParams);
    }
}

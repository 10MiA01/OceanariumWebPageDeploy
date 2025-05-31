using Oceanarium.Models;
using Oceanarium.ViewModels;

namespace Oceanarium.Servises.Interfaces
{
    public interface IFilterExibitionService
    {
        Task<List<Exibition>> GetFilteredAsync(ExibitionFilterParams filterParams);

    }
}

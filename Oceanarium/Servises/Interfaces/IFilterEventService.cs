using Oceanarium.Models;
using Oceanarium.ViewModels;

namespace Oceanarium.Servises.Interfaces
{
    public interface IFilterEventService
    {
        Task<List<Event>> GetFilteredAsync(EventFilterParams filterParams);
    }
}

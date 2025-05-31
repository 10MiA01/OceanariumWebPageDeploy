using Oceanarium.Models;
using Oceanarium.ViewModels;

namespace Oceanarium.Servises.Interfaces
{
    public interface IFilterTicketService
    {
        Task<List<Ticket>> GetFilteredAsync(TicketFilterParams filterParams);
    }
}

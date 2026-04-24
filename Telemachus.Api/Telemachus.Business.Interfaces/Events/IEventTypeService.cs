using System.Collections.Generic;
using System.Threading.Tasks;
using Telemachus.Business.Models.Events;

namespace Telemachus.Business.Interfaces.Events
{
    public interface IEventTypeService
    {
        Task<List<EventTypeBusinessModel>> GetEventTypesAsync();
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Models.Events;

namespace WebApi.Data
{
    public interface IEventRepository
    {
        Task<Event> GetEventByIdAsync(Guid eventId);
        Task<IEnumerable<Event>> GetAllEventsForPartnerAsync(Guid partnerId);
        Task<IEnumerable<Event>> GetAllEventsAsync();
        Task<Event> SaveEventAsync(EventWriteModel eventWriteModel);
        Task<bool> UpdateEventAsync(Guid eventId, EventWriteModel eventWriteModel);
        Task<bool> DeleteEventAsync(Guid eventId);
    }
}

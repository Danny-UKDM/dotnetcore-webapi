﻿using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Models;

namespace WebApi.Controllers.Services
{
    public interface IEventRepository
    {
        Task<IEnumerable<Event>> GetAllEventsAsync();
        Task<Event> GetEventByIdAsync(int id);
    }
}

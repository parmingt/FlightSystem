using FlightSystem.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightSystem.Services;

public class BookingService
{
    public Task BookFlight(Flight selectedFlight)
    {
        return Task.CompletedTask;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightSystem.Services.Models;

public record BookedFlightSummary(DateTime BookingDate, string BookingId, IataCode Origin, IataCode Destination, Price Price);

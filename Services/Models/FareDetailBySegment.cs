using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightSystem.Services.Models;

public record FareDetailBySegment(string SegmentId, string Cabin, string FareBasis, string BrandedFare, string Class, int IncludedCheckedBags)
{
}

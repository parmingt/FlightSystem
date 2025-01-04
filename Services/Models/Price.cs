using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightSystem.Services.Models;

public record Price(decimal Total, string Currency)
{
    public override string ToString()
    {
        return $"{Total} {Currency}";
    }
};

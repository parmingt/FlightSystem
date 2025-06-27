using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightSystem.Kafka.Models;

internal class DataWrapper<T>
{
    public DataWrapper(T data)
    {
        this.data = data;
    }

    public T data { get; set; }
}

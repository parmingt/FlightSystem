using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightSystem.Kafka.Models;

public class KafkaConfiguration
{
    public string BootstrapServers { get; set; }
    public string BookingTopic { get; set; } = "flight-orders";
}

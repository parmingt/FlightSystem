using Confluent.Kafka;
using FlightSystem.Kafka.Models;
using FlightSystem.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightSystem.BookingListener;

public class BookingListener(IConsumer<string, FlightOrder> consumer)
{
    public void Run(CancellationToken cancellationToken)
    {
        consumer.Subscribe("flight-orders");
        while (!cancellationToken.IsCancellationRequested)
        {
            var consumeResult = consumer.Consume();
            Console.WriteLine($"Received message at {consumeResult.TopicPartitionOffset}: {consumeResult.Message.Value}");
            // Simulate processing the booking
            Console.WriteLine("Processing booking...");
            Thread.Sleep(2000); // Simulate some work
            Console.WriteLine("Booking processed.");
        }
    }
}

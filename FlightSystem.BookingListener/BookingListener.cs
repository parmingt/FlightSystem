using Confluent.Kafka;
using FlightSystem.Kafka.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightSystem.BookingListener;

public class BookingListener(KafkaConfiguration kafkaConfiguration)
{
    public void Run()
    {
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = kafkaConfiguration.BootstrapServers,
            GroupId = "booking-engine",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using (var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build())
        {
            consumer.Subscribe("flight-orders");
            while (true)
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
}

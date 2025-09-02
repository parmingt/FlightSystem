// See https://aka.ms/new-console-template for more information
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

var configuration = new ConfigurationBuilder()
     .AddJsonFile($"appsettings.json");

var config = configuration.Build();

var consumerConfig = new ConsumerConfig
{
    BootstrapServers = config
        .GetRequiredSection("Kafka")["BootstrapServers"],
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
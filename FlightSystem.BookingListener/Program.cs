// See https://aka.ms/new-console-template for more information
using Confluent.Kafka;

Console.WriteLine("Hello, World!");

var config = new ConsumerConfig
{
    BootstrapServers = "localhost:19092",
    GroupId = "booking-engine",
    AutoOffsetReset = AutoOffsetReset.Earliest
};

using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
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
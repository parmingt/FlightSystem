using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using FlightSystem.Kafka.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.Kafka;

namespace FlightSystem.BookingListener.Tests;

[TestClass]
public sealed class ListenerTests
{
    private IProducer<string, FlightOrder> _producer;

    [TestMethod]
    public async Task ConsumesMessage()
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json").Build();
        var serviceProvider = new ServiceCollection()
            .AddLogging()
            .AddSingleton<BookingListener>()
            .AddBookingConsumer(configuration)
            .BuildServiceProvider();

        var kafkaContainer = new KafkaBuilder()
          .WithImage("confluentinc/cp-kafka:6.2.10")
          .WithPortBinding(9092)
          .Build();
        await kafkaContainer.StartAsync();

        var bootstrapServers = kafkaContainer.GetBootstrapAddress();
        var schemaRegistry = serviceProvider.GetRequiredService<ISchemaRegistryClient>();
        _producer = new ProducerBuilder<string, FlightOrder>(new ProducerConfig
        {
            BootstrapServers = bootstrapServers
        }).SetValueSerializer(new JsonSerializer<FlightOrder>(schemaRegistry))
        .Build();
        await _producer.ProduceAsync("flight-orders", new Message<string, FlightOrder>()
        {
            Key = "flight-order",
            Value = new FlightOrder()
            {
                flightOffers = Array.Empty<OffersSearch.Offers>()
            }
        });

        var token = new CancellationTokenSource();
        token.CancelAfter(TimeSpan.FromSeconds(2));

        serviceProvider.GetRequiredService<BookingListener>().Run(token.Token);

        var consumer = serviceProvider.GetRequiredService<IConsumer<string, FlightOrder>>();
        Assert.IsTrue(consumer.Subscription.Contains("flight-orders"));
    }
}

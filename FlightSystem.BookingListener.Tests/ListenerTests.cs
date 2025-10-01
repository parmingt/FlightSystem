using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using FlightSystem.Kafka.Models;
using FlightSystem.Services.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.Kafka;

namespace FlightSystem.BookingListener.Tests;

[TestClass]
public sealed class ListenerTests
{
    private IProducer<string, FlightOrder> _producer;
    private ServiceProvider _serviceProvider;

    [TestInitialize]
    public async Task Setup()
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json").Build();
        _serviceProvider = new ServiceCollection()
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
        var schemaRegistry = _serviceProvider.GetRequiredService<ISchemaRegistryClient>();
        _producer = new ProducerBuilder<string, FlightOrder>(new ProducerConfig
        {
            BootstrapServers = bootstrapServers
        }).SetValueSerializer(new JsonSerializer<FlightOrder>(schemaRegistry))
        .Build();
    }

    [TestMethod]
    public async Task ConsumesMessage()
    {
        await _producer.ProduceAsync("flight-orders", new Message<string, FlightOrder>()
        {
            Key = "flight-order",
            Value = new FlightOrder()
            {
                flightOffers = Array.Empty<FlightOffer>()
            }
        });

        var token = new CancellationTokenSource();
        token.CancelAfter(TimeSpan.FromSeconds(2));

        _serviceProvider.GetRequiredService<BookingListener>().Run(token.Token);

        var consumer = _serviceProvider.GetRequiredService<IConsumer<string, FlightOrder>>();
        Assert.IsTrue(consumer.Subscription.Contains("flight-orders"));
    }
}

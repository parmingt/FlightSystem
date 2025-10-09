using AmadeusSDK;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Docker.DotNet.Models;
using DotNet.Testcontainers.Builders;
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
            .AddSingleton<IAmadeusClient, AmadeusClient>()
            .BuildServiceProvider();

        var _kafkaNetwork = new NetworkBuilder().WithName(Guid.NewGuid().ToString("D")).Build();
        _kafkaNetwork.CreateAsync().Wait();

        var kafkaContainer = new KafkaBuilder()
          .WithImage("confluentinc/cp-kafka:6.2.10")
          .WithPortBinding(19092)
          .WithNetwork(_kafkaNetwork)
          .WithNetworkAliases("kafka")
          .WithListener("kafka:19092")
          .Build();
        await kafkaContainer.StartAsync();

        var bootstrapServers = kafkaContainer.GetBootstrapAddress();
        var schemaRegistryContainer = new ContainerBuilder()
          .WithImage("confluentinc/cp-schema-registry:7.5.2")
          .WithNetwork(_kafkaNetwork)
          .WithPortBinding(18083)
          .WithEnvironment("SCHEMA_REGISTRY_HOST_NAME", "schema-registry")
          .WithEnvironment("SCHEMA_REGISTRY_LISTENERS", "http://0.0.0.0:18083")
          .WithEnvironment("SCHEMA_REGISTRY_KAFKASTORE_BOOTSTRAP_SERVERS", $"PLAINTEXT://kafka:19092")
          .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("Server started, listening for requests..."))
          .Build();
        await schemaRegistryContainer.StartAsync();

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
        var offer = new FlightOffer(DateTime.Now, new Price(10, "USD"), [ 
            new Segment("123", "123", new IataCode("PHI"), new IataCode("SLC"), DateTime.Now, "1")
        ], "123", [], "test", []);
        await _producer.ProduceAsync("flight-orders", new Message<string, FlightOrder>()
        {
            Key = "flight-order",
            Value = new FlightOrder()
            {
                
                flightOffers = [  ],  
            }
        });

        var token = new CancellationTokenSource();
        token.CancelAfter(TimeSpan.FromSeconds(2));

        _serviceProvider.GetRequiredService<BookingListener>().Run(token.Token);

        var consumer = _serviceProvider.GetRequiredService<IConsumer<string, FlightOrder>>();
        Assert.IsTrue(consumer.Subscription.Contains("flight-orders"));
    }
}

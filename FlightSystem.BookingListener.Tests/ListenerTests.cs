using AmadeusSDK;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Docker.DotNet.Models;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using FlightSystem.BookingListener.Tests.Fakes;
using FlightSystem.Kafka.Models;
using FlightSystem.Services.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Testcontainers.Kafka;

namespace FlightSystem.BookingListener.Tests;

[TestClass]
public sealed class ListenerTests
{
    private IProducer<string, FlightOrder> _producer;
    private ServiceProvider _serviceProvider;
    private AmadeusClientFake _amadeusClientFake = new();
    private KafkaContainer _kafkaContainer;
    private IContainer _schemaRegistryContainer;

    [TestInitialize]
    public async Task Setup()
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json").Build();
        _serviceProvider = new ServiceCollection()
            .AddLogging()
            .AddSingleton<BookingListener>()
            .AddBookingConsumer(configuration)
            .AddSingleton<IAmadeusClient>(_amadeusClientFake)
            .AddMemoryCache()
            .BuildServiceProvider();

        var testContainerNetwork = new NetworkBuilder().Build();
        await testContainerNetwork.CreateAsync();

        _kafkaContainer = new KafkaBuilder()
          .WithImage("confluentinc/cp-kafka:6.2.10")
          .WithNetwork(testContainerNetwork)
          .WithNetworkAliases("kafka")
          .WithListener("kafka:19092")
          .Build();
        await _kafkaContainer.StartAsync();

        var bootstrapServers = _kafkaContainer.GetBootstrapAddress();
        _schemaRegistryContainer = new ContainerBuilder()
          .WithImage("confluentinc/cp-schema-registry:7.5.2")
          .WithNetwork(testContainerNetwork)
          .WithPortBinding(18083, true)
          .WithEnvironment("SCHEMA_REGISTRY_HOST_NAME", "schema-registry")
          .WithEnvironment("SCHEMA_REGISTRY_LISTENERS", "http://0.0.0.0:18083")
          .WithEnvironment("SCHEMA_REGISTRY_KAFKASTORE_BOOTSTRAP_SERVERS", $"PLAINTEXT://kafka:19092")
          .WithWaitStrategy(Wait.ForUnixContainer().UntilInternalTcpPortIsAvailable(18083))
          .Build();
        await _schemaRegistryContainer.StartAsync();

        var schemaRegistryPort = _schemaRegistryContainer.GetMappedPublicPort();
        _serviceProvider.GetRequiredService<Kafka.Models.KafkaConfiguration>().BootstrapServers = bootstrapServers;
        configuration["SchemaRegistry:Url"] = $"http://localhost:{schemaRegistryPort}";
        var schemaRegistry = _serviceProvider.GetRequiredService<ISchemaRegistryClient>();
        _producer = new ProducerBuilder<string, FlightOrder>(new ProducerConfig
        {
            BootstrapServers = bootstrapServers
        }).SetValueSerializer(new JsonSerializer<FlightOrder>(schemaRegistry))
        .Build();
    }

    [TestCleanup]
    public async Task Cleanup()
    {
        _producer.Dispose();
        _serviceProvider.Dispose();
        await _kafkaContainer.DisposeAsync();
        await _schemaRegistryContainer.DisposeAsync();
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
                flightOffers = [ offer ]
            }
        });

        var token = new CancellationTokenSource(1000);

        try
        {
            await _serviceProvider.GetRequiredService<BookingListener>().Run(token.Token);
        }
        catch (OperationCanceledException){}

        var consumer = _serviceProvider.GetRequiredService<IConsumer<string, FlightOrder>>();
        Assert.IsTrue(consumer.Subscription.Contains("flight-orders"));
        Assert.AreEqual(1, _amadeusClientFake.BookedOffers.Count);
        Assert.AreEqual("10.0", _amadeusClientFake.BookedOffers[0].price.total);
    }
}

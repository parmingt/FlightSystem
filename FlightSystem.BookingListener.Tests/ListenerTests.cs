using AmadeusSDK;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using FlightSystem.BookingListener.Tests.Fakes;
using FlightSystem.Kafka.Models;
using FlightSystem.Services.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using Testcontainers.Kafka;

namespace FlightSystem.BookingListener.Tests;

[TestClass]
public sealed class ListenerTests
{
    private IProducer<string, FlightOrder> _producer;
    private ServiceProvider _serviceProvider;
    private AmadeusClientFake _amadeusClientFake;
    private static KafkaContainer _kafkaContainer;
    private static IContainer _schemaRegistryContainer;

    [ClassInitialize]
    public static async Task Setup(TestContext testContext)
    {
        var brokerInternalPort = 19092;
        var schemaRegistryInternalPort = 18083;

        var testContainerNetwork = new NetworkBuilder().Build();
        await testContainerNetwork.CreateAsync();

        _kafkaContainer = new KafkaBuilder()
          .WithImage("confluentinc/cp-kafka:6.2.10")
          .WithNetwork(testContainerNetwork)
          .WithNetworkAliases("kafka")
          .WithListener($"kafka:{brokerInternalPort}")
          .Build();
        await _kafkaContainer.StartAsync();

        _schemaRegistryContainer = new ContainerBuilder()
          .WithImage("confluentinc/cp-schema-registry:7.5.2")
          .WithNetwork(testContainerNetwork)
          .WithPortBinding(schemaRegistryInternalPort, true)
          .WithEnvironment("SCHEMA_REGISTRY_HOST_NAME", "schema-registry")
          .WithEnvironment("SCHEMA_REGISTRY_LISTENERS", $"http://0.0.0.0:{schemaRegistryInternalPort}")
          .WithEnvironment("SCHEMA_REGISTRY_KAFKASTORE_BOOTSTRAP_SERVERS", $"PLAINTEXT://kafka:{brokerInternalPort}")
          .WithWaitStrategy(Wait.ForUnixContainer().UntilInternalTcpPortIsAvailable(schemaRegistryInternalPort))
          .Build();
        await _schemaRegistryContainer.StartAsync();
    }

    [TestInitialize]
    public async Task TestSetup()
    {
        _amadeusClientFake = new();
        IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json").Build();
        _serviceProvider = new ServiceCollection()
            .AddLogging()
            .AddSingleton<BookingListener>()
            .AddBookingConsumer(configuration)
            .AddSingleton<IAmadeusClient>(_amadeusClientFake)
            .AddMemoryCache()
            .BuildServiceProvider();

        var bookingTopic = $"flight-orders-{Guid.NewGuid()}";
        var bootstrapServers = _kafkaContainer.GetBootstrapAddress();
        var schemaRegistryPort = _schemaRegistryContainer.GetMappedPublicPort();
        var schemaRegistryUrl = $"http://localhost:{schemaRegistryPort}";

        _serviceProvider.GetRequiredService<Kafka.Models.KafkaConfiguration>().BootstrapServers = bootstrapServers;
        _serviceProvider.GetRequiredService<Kafka.Models.KafkaConfiguration>().BookingTopic = bookingTopic;
        _serviceProvider.GetRequiredService<Kafka.Models.KafkaConfiguration>().BookingConsumerGroupId = $"booking-engine-{Guid.NewGuid()}";
        configuration["SchemaRegistry:Url"] = $"http://localhost:{schemaRegistryPort}";
        var schemaRegistry = _serviceProvider.GetRequiredService<ISchemaRegistryClient>();
        _producer = new ProducerBuilder<string, FlightOrder>(new ProducerConfig
        {
            BootstrapServers = bootstrapServers
        }).SetValueSerializer(new JsonSerializer<FlightOrder>(schemaRegistry))
        .Build();

        var offer = new FlightOffer(DateTime.Now, new Price(10, "USD"), [
            new Segment("123", "123", new IataCode("PHI"), new IataCode("SLC"), DateTime.Now, "1")
        ], "123", [], "test", []);

        var result = await _producer.ProduceAsync(bookingTopic, new Message<string, FlightOrder>()
        {
            Key = "flight-order",
            Value = new FlightOrder()
            {
                flightOffers = [offer]
            }
        });
        Debug.WriteLine(result.Offset);
    }

    [ClassCleanup]
    public static async Task Cleanup()
    {
        await _kafkaContainer.DisposeAsync();
        await _schemaRegistryContainer.DisposeAsync();
    }

    [TestCleanup]
    public void TestCleanup()
    {
        //_producer?.Dispose();
        //_serviceProvider.Dispose();
        //var config = new AdminClientConfig { BootstrapServers = _kafkaContainer.GetBootstrapAddress() };
        //using var adminClient = new AdminClientBuilder(config).Build();
        // adminClient.DeleteGroupsAsync([ "booking-engine" ]).GetAwaiter().GetResult();
    }

    record Reservation(DateTime Departure, string FlightNumber, Guid BookingReference);

    // [TestMethod]
    public async Task BrokerIsRunning()
    {
        var bookingTopic = "flight-orders";

        var bootstrapServers = _kafkaContainer.GetBootstrapAddress();

        var schemaRegistryConfig = new SchemaRegistryConfig()
        {
            Url = $"http://localhost:{_schemaRegistryContainer.GetMappedPublicPort()}"
        };
        var schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig);

        using var producer = new ProducerBuilder<string, Reservation>(new ProducerConfig
        {
            BootstrapServers = bootstrapServers
        })
        .SetValueSerializer(new JsonSerializer<Reservation>(schemaRegistry))
        .Build();

        using var consumer = new ConsumerBuilder<string, Reservation>(new ConsumerConfig
        {
            BootstrapServers = bootstrapServers,
            GroupId = "test-group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        })
        .SetValueDeserializer(new JsonDeserializer<Reservation>(schemaRegistry).AsSyncOverAsync())
        .Build();

        var reservation = new Reservation(DateTime.Now, "123", Guid.NewGuid());

        await producer.ProduceAsync(bookingTopic, new Message<string, Reservation>()
        {
            Key = reservation.BookingReference.ToString(),
            Value = reservation
        });

        consumer.Subscribe(bookingTopic);
        var consumeResult = consumer.Consume(TimeSpan.FromMilliseconds(500));

        Assert.AreEqual(reservation, consumeResult.Message.Value);
    }

    [TestMethod]
    public async Task ConsumesMessage()
    {
        var token = new CancellationTokenSource(1000);

        try
        {
            await _serviceProvider.GetRequiredService<BookingListener>().Run(token.Token);
        }
        catch (OperationCanceledException){ }
        catch (Exception ex)
        {
            Assert.Fail(ex.Message);
        }

        Assert.AreEqual(1, _amadeusClientFake.BookedOffers.Count);
        Assert.AreEqual("10.0", _amadeusClientFake.BookedOffers[0].price.total);
    }

    [TestMethod]
    public async Task ConsumesMessage2()
    {
        var token = new CancellationTokenSource(1000);

        try
        {
            await _serviceProvider.GetRequiredService<BookingListener>().Run(token.Token);
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            Assert.Fail(ex.Message);
        }

        Assert.AreEqual(1, _amadeusClientFake.BookedOffers.Count);
        Assert.AreEqual("10.0", _amadeusClientFake.BookedOffers[0].price.total);
    }
}

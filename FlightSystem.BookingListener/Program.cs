using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using FlightSystem.BookingListener;
using FlightSystem.Kafka.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

IConfiguration configuration = new ConfigurationBuilder()
     .AddJsonFile("appsettings.json").Build();

var serviceProvider = new ServiceCollection()
    .AddLogging()
    .AddSingleton<BookingListener>()
    .AddSingleton(_ => {
        var kafkaConfig = new KafkaConfiguration();
        configuration.Bind("Kafka", kafkaConfig);
        return kafkaConfig;
    })
    .AddSingleton<ISchemaRegistryClient>(sp =>
    {
        var schemaRegistryConfig = new SchemaRegistryConfig();
        configuration.Bind("SchemaRegistry", schemaRegistryConfig);
        return new CachedSchemaRegistryClient(schemaRegistryConfig);
    })
    .AddSingleton(sp =>
    {
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = sp.GetRequiredService<KafkaConfiguration>().BootstrapServers,
            GroupId = "booking-engine",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        var schemaRegistry = sp.GetRequiredService<ISchemaRegistryClient>();
        var consumerBuilder = new ConsumerBuilder<string, FlightOrder>(consumerConfig);
        consumerBuilder.SetValueDeserializer(new JsonDeserializer<FlightOrder>(schemaRegistry).AsSyncOverAsync());
        return consumerBuilder.Build();
    })
    .BuildServiceProvider();

serviceProvider.GetRequiredService<BookingListener>().Run();
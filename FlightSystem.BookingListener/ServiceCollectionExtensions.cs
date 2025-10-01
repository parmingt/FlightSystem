using Confluent.Kafka;
using Confluent.SchemaRegistry.Serdes;
using Confluent.SchemaRegistry;
using FlightSystem.Kafka.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Confluent.Kafka.SyncOverAsync;
using FlightSystem.Services.Models;

namespace FlightSystem.BookingListener;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBookingConsumer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(_ =>
        {
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
        });
        return services;
    }
}

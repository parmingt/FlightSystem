using Confluent.Kafka;
using FlightSystem.BookingListener;
using FlightSystem.Kafka.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

IConfiguration configuration = new ConfigurationBuilder()
     .AddJsonFile($"appsettings.json").Build();

var serviceProvider = new ServiceCollection()
    .AddLogging()
    .AddSingleton<BookingListener>()
    .AddSingleton(_ => {
        var kafkaConfig = new KafkaConfiguration();
        configuration.Bind("Kafka", kafkaConfig);
        return kafkaConfig;
    })
    .BuildServiceProvider();

serviceProvider.GetRequiredService<BookingListener>().Run();
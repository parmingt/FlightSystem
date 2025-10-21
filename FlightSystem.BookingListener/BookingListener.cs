using AmadeusSDK;
using Confluent.Kafka;
using FlightSystem.Kafka.Models;
using FlightSystem.Services;
using FlightSystem.Services.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightSystem.BookingListener;

public class BookingListener(IConsumer<string, FlightOrder> consumer, IAmadeusClient amadeusClient, KafkaConfiguration kafkaConfiguration)
{
    public async Task Run(CancellationToken cancellationToken)
    {
        consumer.Subscribe(kafkaConfiguration.BookingTopic);

        while (!cancellationToken.IsCancellationRequested)
        {
            var consumeResult = consumer.Consume(cancellationToken);
            await amadeusClient.BookFlight(new AmadeusSDK.Models.FlightOrder() 
            { 
                flightOffers = consumeResult.Message.Value.flightOffers.Select(fo => fo.ToOffer()).ToList() 
            });
            Console.WriteLine("Booking processed.");
        }
    }
}

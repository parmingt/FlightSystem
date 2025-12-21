using AmadeusSDK;
using AmadeusSDK.Models;
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
using FlightOrder = FlightSystem.Services.Models.FlightOrder;

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
                FlightOffers = consumeResult.Message.Value.flightOffers.Select(fo => fo.ToOffer()).ToList(),
                Travelers = [
                    new Traveler(){
                        Id = "1",
                        Name = new Name(){
                            FirstName = "John",
                            LastName = "Doe"
                        },
                        Contact = new Contact() {
                            EmailAddress = "test@gmail.com"
                        },
                        Gender = "MALE",
                        DateOfBirth = "1990-01-01"
                    }
                ]
            });
            Console.WriteLine("Booking processed.");
        }
    }
}

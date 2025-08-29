using Confluent.Kafka;
using Testcontainers.Kafka;

namespace FlightSystem.BookingListener.Tests;

[TestClass]
public sealed class ListenerTests
{
    private IProducer<Null, string> _producer;

    [TestMethod]
    public async Task ConsumesMessage()
    {
        var kafkaContainer = new KafkaBuilder()
          .WithImage("confluentinc/cp-kafka:6.2.10")
          .WithPortBinding(9092)
          .Build();
        await kafkaContainer.StartAsync();

        var bootstrapServers = kafkaContainer.GetBootstrapAddress();
        _producer = new ProducerBuilder<Null, string>(new ProducerConfig
        {
            BootstrapServers = bootstrapServers
        }).Build();
        await _producer.ProduceAsync("test-topic", new Message<Null, string>()
        {
            Value = "Hello"
        });

        var consumer = new ConsumerBuilder<Null, string>(new ConsumerConfig
        {
            BootstrapServers = bootstrapServers,
            GroupId = Guid.NewGuid().ToString(),
            AutoOffsetReset = AutoOffsetReset.Earliest
        }).Build();
        consumer.Subscribe("test-topic");
        var message = consumer.Consume();
        Assert.AreEqual(message.Message.Value, "Hello");
    }
}

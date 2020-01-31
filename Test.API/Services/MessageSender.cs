namespace Test.API.Services
{
    using System;
    using System.Text;
    using RabbitMQ.Client;
    public class MessageSender : IMessageSender
    {
        public void Send(string msg)
        {
            var factory = new ConnectionFactory() { HostName = "rabbitmq" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare("logs", ExchangeType.Fanout, durable: true);

                var body = Encoding.UTF8.GetBytes(msg);

                channel.BasicPublish(exchange: "logs",
                     routingKey: "",
                     basicProperties: null,
                     body: body);
                Console.WriteLine($"[x] Sent {msg}");
            }
        }
    }
}
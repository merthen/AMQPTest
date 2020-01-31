namespace Test.ListenerService
{
    using System;
    using System.Text;
    using System.Threading;
    using Microsoft.Extensions.Logging;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;

    public class Listener
    {
        private readonly ILogger<Listener> _logger;
        public Listener(ILogger<Listener> logger)
        {
            _logger = logger;

        }
        public void Listen()
        {


            var factory = new ConnectionFactory() { HostName = "rabbitmq" };
            factory.AutomaticRecoveryEnabled = true;
            factory.NetworkRecoveryInterval = TimeSpan.FromSeconds(10);
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "logs", type: ExchangeType.Fanout, durable: true);

                var queueName = channel.QueueDeclare().QueueName;
                channel.QueueBind(queue: queueName,
                                  exchange: "logs",
                                  routingKey: "");

                Console.WriteLine(" [*] Waiting for logs.");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    _logger.LogInformation(message);
                    Console.WriteLine($" [x] {message}");
                };
                channel.BasicConsume(queue: queueName,
                                     autoAck: true,
                                     consumer: consumer);
                Console.WriteLine(" Press [enter] to exit.");
                Thread.Sleep(Timeout.Infinite);
            }
        }

    }
}

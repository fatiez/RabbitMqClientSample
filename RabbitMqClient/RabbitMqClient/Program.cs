using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;


namespace RabbitMqClient
{
    class Program
    {
        static IConnection conn;
        static IModel channel;

        static void Main(string[] args)
        {
            ConnectionFactory factory = new ConnectionFactory()
            {
                HostName = "localhost",
                VirtualHost = "/",
                Port = 5672,
                UserName = "guest",
                Password = "guest"
            };

            conn = factory.CreateConnection();
            channel = conn.CreateModel();

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += Consumer_Received;

            var consumerTag = channel.BasicConsume("my.queue1", false, consumer);

            Console.WriteLine("Wating for messages. Press any key to exit.");
            Console.ReadKey();
        }

        private static void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            string message = Encoding.UTF8.GetString(e.Body.ToArray());
            Console.WriteLine($"Message: {message}");
            channel.BasicAck(e.DeliveryTag, true);
            channel.BasicNack(e.DeliveryTag, true, true);
        }
    }
}

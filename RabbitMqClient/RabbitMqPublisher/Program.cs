using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace RabbitMqPublisher
{

    class Program
    {
        static void Main(string[] args)
        {
            FanoutPublisherDemo();
            TopicPublisherDemo();
            DirectPublisherDemo();
            HeaderPublisherDemo();
            DefaultPublisherDemo();
        }

        public static void FanoutPublisherDemo()
        {
            ConnectionFactory factory = new ConnectionFactory()
            {
                HostName = "localhost",
                VirtualHost = "/",
                Port = 5672,
                UserName = "guest",
                Password = "guest"
            };

            using (IConnection conn = factory.CreateConnection())
            using (IModel channel = conn.CreateModel())
            {
                channel.ExchangeDeclare("ex.fanout", "fanout", true, false, null);
                channel.QueueDeclare("my.queue1", true, false, false, null);
                channel.QueueDeclare("my.queue2", true, false, false, null);

                channel.QueueBind("my.queue1", "ex.fanout", "");
                channel.QueueBind("my.queue2", "ex.fanout", "");

                channel.BasicPublish("ex.fanout", "", false, null, Encoding.UTF8.GetBytes("Merhaba Dünya!"));
                channel.BasicPublish("ex.fanout", "", false, null, Encoding.UTF8.GetBytes("Merhaba RabbitMQ!"));

                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();

                channel.QueueDelete("my.queue1");
                channel.QueueDelete("my.queue2");
                channel.ExchangeDelete("ex.fanout");

                channel.Close();
                conn.Close();

            }
        }

        public static void TopicPublisherDemo()
        {
            IConnection conn;
            IModel channel;

            ConnectionFactory factory = new ConnectionFactory();
            // "guest"/"guest" by default, limited to localhost connections
            factory.HostName = "localhost";
            factory.VirtualHost = "/";
            factory.Port = 5672;
            factory.UserName = "guest";
            factory.Password = "guest";

            conn = factory.CreateConnection();
            channel = conn.CreateModel();

            channel.ExchangeDeclare(
                "ex.topic",
                "topic",
                true,
                false,
                null);

            channel.QueueDeclare(
                "my.queue1",
                true,
                false,
                false,
                null);

            channel.QueueDeclare(
                "my.queue2",
                true,
                false,
                false,
                null);

            channel.QueueDeclare(
                "my.queue3",
                true,
                false,
                false,
                null);

            channel.QueueBind("my.queue1", "ex.topic", "*.image.*");
            channel.QueueBind("my.queue2", "ex.topic", "#.image");
            channel.QueueBind("my.queue3", "ex.topic", "image.#");

            channel.BasicPublish(
                "ex.topic",
                "convert.image.bmp",
                null,
                Encoding.UTF8.GetBytes("Routing key is convert.image.bmp"));

            channel.BasicPublish(
                "ex.topic",
                "convert.bitmap.image",
                null,
                Encoding.UTF8.GetBytes("Routing key is convert.bitmap.image"));

            channel.BasicPublish(
                "ex.topic",
                "image.bitmap.32bit",
                null,
                Encoding.UTF8.GetBytes("Routing key is image.bitmap.32bit"));

        }

        public static void DirectPublisherDemo()
        {
            IConnection conn;
            IModel channel;

            ConnectionFactory factory = new ConnectionFactory();
            // "guest"/"guest" by default, limited to localhost connections
            factory.HostName = "localhost";
            factory.VirtualHost = "/";
            factory.Port = 5672;
            factory.UserName = "guest";
            factory.Password = "guest";

            conn = factory.CreateConnection();
            channel = conn.CreateModel();

            channel.ExchangeDeclare
                (
                "ex.direct",
                "direct",
                true,
                false,
                null);

            channel.QueueDeclare(
                "my.infos",
                true,
                false,
                false,
                null);

            channel.QueueDeclare(
                "my.warnings",
                true,
                false,
                false,
                null);

            channel.QueueDeclare(
                "my.errors",
                true,
                false,
                false,
                null);

            channel.QueueBind("my.infos", "ex.direct", "info");
            channel.QueueBind("my.warnings", "ex.direct", "warning");
            channel.QueueBind("my.errors", "ex.direct", "error");

            channel.BasicPublish(
                "ex.direct",
                "info",
                null,
                Encoding.UTF8.GetBytes("Message with routing key info."));

            channel.BasicPublish(
                "ex.direct",
                "warning",
                null,
                Encoding.UTF8.GetBytes("Message with routing key warning."));

            channel.BasicPublish(
                "ex.direct",
                "error",
                null,
                Encoding.UTF8.GetBytes("Message with routing key error."));

        }

        public static void HeaderPublisherDemo()
        {
            IConnection conn;
            IModel channel;

            ConnectionFactory factory = new ConnectionFactory();
            // "guest"/"guest" by default, limited to localhost connections
            factory.HostName = "localhost";
            factory.VirtualHost = "/";
            factory.Port = 5672;
            factory.UserName = "guest";
            factory.Password = "guest";

            conn = factory.CreateConnection();
            channel = conn.CreateModel();

            channel.ExchangeDeclare(
                "ex.headers",
                "headers",
                true,
                false,
                null);

            channel.QueueDeclare(
                "my.queue1",
                true,
                false,
                false,
                null);

            channel.QueueDeclare(
                "my.queue2",
                true,
                false,
                false,
                null);

            channel.QueueBind(
                "my.queue1",
                "ex.headers",
                "",
                new Dictionary<string, object>()
                {
                    {"x-match","all" },
                    {"job","convert" },
                    {"format","jpeg" }
                });

            channel.QueueBind(
                "my.queue2",
                "ex.headers",
                "",
                new Dictionary<string, object>()
                {
                    {"x-match","any" },
                    {"job","convert" },
                    {"format","jpeg" }
                });

            IBasicProperties props = channel.CreateBasicProperties();
            props.Headers = new Dictionary<string, object>();
            props.Headers.Add("job", "convert");
            props.Headers.Add("format", "jpeg");

            channel.BasicPublish(
                "ex.headers",
                "",
                props,
                Encoding.UTF8.GetBytes("Message 1"));

            props = channel.CreateBasicProperties();
            props.Headers = new Dictionary<string, object>();
            props.Headers.Add("job", "convert");
            props.Headers.Add("format", "bmp");

            channel.BasicPublish(
                "ex.headers",
                "",
                props,
                Encoding.UTF8.GetBytes("Message 2"));

        }

        public static void DefaultPublisherDemo()
        {
            IConnection conn;
            IModel channel;

            ConnectionFactory factory = new ConnectionFactory();
            // "guest"/"guest" by default, limited to localhost connections
            factory.HostName = "localhost";
            factory.VirtualHost = "/";
            factory.Port = 5672;
            factory.UserName = "guest";
            factory.Password = "guest";

            conn = factory.CreateConnection();
            channel = conn.CreateModel();

            channel.QueueDeclare(
                "my.queue1",
                true,
                false,
                false,
                null);

            channel.QueueDeclare(
                "my.queue2",
                true,
                false,
                false,
                null);

            channel.BasicPublish(
                "",
                "my.queue1",
                null,
                Encoding.UTF8.GetBytes("Message with routing key my.queue1"));

            channel.BasicPublish(
                "",
                "my.queue2",
                null,
                Encoding.UTF8.GetBytes("Message with routing key my.queue2"));
        }

        public static void Exchange2ExchangeBinding()
        {


            IConnection conn;
            IModel channel;

            ConnectionFactory factory = new ConnectionFactory();
            // "guest"/"guest" by default, limited to localhost connections
            factory.HostName = "localhost";
            factory.VirtualHost = "/";
            factory.Port = 5672;
            factory.UserName = "guest";
            factory.Password = "guest";

            conn = factory.CreateConnection();
            channel = conn.CreateModel();

            channel.ExchangeDeclare(
                "exchange1",
                "direct",
                true,
                false,
                null);

            channel.ExchangeDeclare(
                "exchange2",
                "direct",
                true,
                false,
                null);

            channel.QueueDeclare(
                "queue1",
                true,
                false,
                false,
                null);

            channel.QueueDeclare(
                "queue2",
                true,
                false,
                false,
                null);

            channel.QueueBind("queue1", "exchange1", "key1");
            channel.QueueBind("queue2", "exchange2", "key2");

            channel.ExchangeBind("exchange2", "exchange1", "key2");

            channel.BasicPublish(
                "exchange1",
                "key1",
                null,
                Encoding.UTF8.GetBytes("Message with routing key key1"));

            channel.BasicPublish(
                "exchange1",
                "key2",
                null,
                Encoding.UTF8.GetBytes("Message with routing key key2"));
        }

        public static void AlternateExchange()
        {
            IConnection conn;
            IModel channel;

            ConnectionFactory factory = new ConnectionFactory();
            // "guest"/"guest" by default, limited to localhost connections
            factory.HostName = "localhost";
            factory.VirtualHost = "/";
            factory.Port = 5672;
            factory.UserName = "guest";
            factory.Password = "guest";

            conn = factory.CreateConnection();
            channel = conn.CreateModel();

            channel.ExchangeDeclare(
                "ex.fanout",
                "fanout",
                true,
                false,
                null);

            channel.ExchangeDeclare(
                "ex.direct",
                "direct",
                true,
                false,
                new Dictionary<string, object>()
                {
                    { "alternate-exchange", "ex.fanout" }
                });

            channel.QueueDeclare(
                "my.queue1",
                true,
                false,
                false,
                null);

            channel.QueueDeclare(
                "my.queue2",
                true,
                false,
                false,
                null);

            channel.QueueDeclare(
                "my.unrouted",
                true,
                false,
                false,
                null);

            channel.QueueBind("my.queue1", "ex.direct", "video");
            channel.QueueBind("my.queue2", "ex.direct", "image");
            channel.QueueBind("my.unrouted", "ex.fanout", "");

            channel.BasicPublish(
                "ex.direct",
                "video",
                null,
                Encoding.UTF8.GetBytes("Message with routing key video"));

            channel.BasicPublish(
                "ex.direct",
                "text",
                null,
                Encoding.UTF8.GetBytes("Message with routing key text"));
        }
    }
}

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Receive
{
    class Program
    {
        private static List<ulong> _deliveryTag  = new List<ulong>();
        private static int _limit = 50;

        static void Main(string[] args)
        {
            ReceivedMessages();
        }

        private static void ReceivedMessages()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };

            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "orderClient", durable: false, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            var messages = new List<Cliente>();
            var queueLimit = _limit;

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(" [x] Received {0}", message);

                ReadMessage(message, ea, channel, consumer, messages, new Cliente(), queueLimit);

            };

            channel.BasicConsume(queue: "orderClient", autoAck: false, consumer: consumer);

            Console.WriteLine("Press [enter] to exit.");
            Console.ReadLine();
        }

        private static void ReadMessage(
            string message,
            BasicDeliverEventArgs ea,
            IModel channel,
            EventingBasicConsumer consumer,
            List<Cliente> messages,
            Cliente queueMessage,
            int queueLimit)
        {
            try
            {
                _deliveryTag.Add(ea.DeliveryTag);

                queueMessage = JsonSerializer.Deserialize<Cliente>(message);

                messages.Add(queueMessage);

                if (messages.Count == queueLimit)
                {
                    Console.WriteLine("Inicizando o processamento de liberar RABBIT");

                    Parallel.ForEach(_deliveryTag, dt => {
                        channel.BasicAck(deliveryTag: dt, multiple: false);
                    });

                    messages.Clear();
                    _deliveryTag.Clear();
                }

            }
            catch (Exception e)
            {
                var i = e;
            }
        }

        public class Cliente
        {
            public int Id { get; set; }
            public string Nome { get; set; }
            public string Email { get; set; }
            public string Telefone { get; set; }
            public string Endereco { get; set; }
            public DateTime Nascimento { get; set; }
            public string Sexo { get; set; }
            public bool Ativo { get; set; }
            public decimal Renda { get; set; }
        }
    }
}

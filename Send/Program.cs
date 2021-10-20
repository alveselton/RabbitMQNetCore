using Bogus;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Send
{
    class Program
    {
        static void Main(string[] args)
        {
            var listaCliente = ListaClientesFake();

            
            /*Parallel.ForEach(listaCliente, client =>
            {
                enviar(client);
            });*/

            foreach (var item in listaCliente)
            {
                enviar(item);
            }

            Console.WriteLine("Carga finalizada!");

        }

        private static void enviar(Cliente cliente)
        {
            var factory = new ConnectionFactory() { HostName = "localhost", UserName = "guest", Password = "guest" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "orderClient",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                string message = JsonSerializer.Serialize(cliente);
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: "orderClient",
                                     basicProperties: null,
                                     body: body);
                Console.WriteLine(" [x] Sent {0}", message);
            }
        }

        public static List<Cliente> ListaClientesFake()
        {
            var clienteFaker = new Faker<Cliente>("pt_BR")
                .RuleFor(c => c.Id, f => f.IndexFaker)
                .RuleFor(c => c.Nome, f => f.Name.FullName(Bogus.DataSets.Name.Gender.Female))
                .RuleFor(c => c.Email, f => f.Internet.Email(f.Person.FirstName).ToLower())
                .RuleFor(c => c.Telefone, f => f.Person.Phone)
                .RuleFor(c => c.Endereco, f => f.Address.StreetAddress())
                .RuleFor(c => c.Nascimento, f => f.Date.Recent(100))
                .RuleFor(c => c.Sexo, f => f.PickRandom(new string[] { "masculino", "feminino" }))
                .RuleFor(c => c.Ativo, f => f.PickRandomParam(new bool[] { true, true, false }))
                .RuleFor(o => o.Renda, f => f.Random.Decimal(500, 2000));
            var clientes = clienteFaker.Generate(1000);
            return clientes;
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

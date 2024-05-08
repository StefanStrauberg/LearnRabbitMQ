using System.Text;
using RabbitMQ.Client;

Console.WriteLine(new String('-', 5) + "RabbitMQ Receiver App" + new String('-', 5));

var factory = new ConnectionFactory() { HostName = "localhost", UserName = "test", Password = "test" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(queue: "BasicTest", durable: false, autoDelete: false, exclusive: false, arguments: null);

string message = "Getting started with .NET Core RabbitMQ";
var body = Encoding.UTF8.GetBytes(message);

channel.BasicPublish(exchange: "", routingKey: "BasicTest", basicProperties: null, body: body);

Console.WriteLine("Sent message: {0}", message);
Console.WriteLine("Press [Enter] to exit Sender App.");
Console.ReadLine();
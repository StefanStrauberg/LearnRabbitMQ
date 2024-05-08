using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

Console.WriteLine(new String('-', 5) + "RabbitMQ Receiver App" + new String('-', 5));

var factory = new ConnectionFactory() { HostName = "localhost", UserName = "test", Password = "test" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(queue: "BasicTest", durable: false, autoDelete: false, exclusive: false, arguments: null);

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
  var body = ea.Body.ToArray();
  var message = Encoding.UTF8.GetString(body);
  Console.WriteLine("Received message: {0}", message);
};

channel.BasicConsume(queue: "BasicTest", autoAck: true, consumer: consumer);

Console.WriteLine("Press [Enter] to exit Sender App.");
Console.ReadLine();
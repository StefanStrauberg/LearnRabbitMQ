using System.Text;
using MediatR;
using MicroRabbit.Domain.Core.Bus;
using MicroRabbit.Domain.Core.Commands;
using MicroRabbit.Domain.Core.Events;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MicroRabbit.Infra.Bus;

public sealed class RabbitMQBus(IMediator mediator) : IEventBus
{
  readonly IMediator _mediator = mediator
    ?? throw new ArgumentNullException(nameof(mediator));
  readonly Dictionary<string, List<Type>> _handlers = [];
  readonly List<Type> _eventTypes = [];

  public void Publish<T>(T @event) where T : Event
  {
    var factory = new ConnectionFactory()
    {
      HostName = "localhost",
      UserName = "test",
      Password = "test"
    };
    using var connection = factory.CreateConnection();
    using var channel = connection.CreateModel();
    var eventName = @event.GetType().Name;
    channel.QueueDeclare(queue: eventName, durable: false, exclusive: false, autoDelete: false, arguments: null);
    var message = JsonConvert.SerializeObject(@event);
    var body = Encoding.UTF8.GetBytes(message);

    channel.BasicPublish(exchange: "", routingKey: eventName, basicProperties: null, body: body);
  }

  public Task SendCommand<T>(T command) where T : Command
    => _mediator.Send(command);

  public void Subscribe<T, TH>()
      where T : Event
      where TH : IEventHandler<T>
  {
    var eventName = typeof(T).Name;
    var handlerType = typeof(TH);

    if (!_eventTypes.Contains(typeof(T)))
      _eventTypes.Add(typeof(T));

    if (!_handlers.ContainsKey(eventName))
      _handlers.Add(eventName, []);

    if (_handlers[eventName].Any(x => x.GetType() == handlerType))
      throw new ArgumentException($"Handler Type {handlerType.Name} is already refistered for '{eventName}'", nameof(handlerType));

    _handlers[eventName].Add(handlerType);

    StartBasicConsume<T>();
  }

  void StartBasicConsume<T>() where T : Event
  {
    var factory = new ConnectionFactory()
    {
      HostName = "localhost",
      UserName = "test",
      Password = "test",
      DispatchConsumersAsync = true
    };
    var connection = factory.CreateConnection();
    var channel = connection.CreateModel();
    var eventName = typeof(T).Name;
    channel.QueueDeclare(queue: eventName, durable: false, exclusive: false, autoDelete: false, arguments: null);
    var consumer = new AsyncEventingBasicConsumer(channel);
    consumer.Received += Consumer_Received;

    channel.BasicConsume(queue: eventName, autoAck: true, consumer: consumer);
  }

  private async Task Consumer_Received(object sender, BasicDeliverEventArgs e)
  {
    var eventName = e.RoutingKey;
    var message = Encoding.UTF8.GetString(e.Body.ToArray());
    try
    {
      await ProcessEvent(eventName, message).ConfigureAwait(false);
    }
    catch (Exception)
    {
    }
  }

  private async Task ProcessEvent(string eventName, string message)
  {
    if (_handlers.ContainsKey(eventName))
    {
      var subscriptions = _handlers[eventName];
      foreach (var subscription in subscriptions)
      {
        var handler = Activator.CreateInstance(subscription);
        if (handler is null)
          continue;
        var eventType = _eventTypes.SingleOrDefault(x => x.Name == eventName);
        var @event = JsonConvert.DeserializeObject(message, eventType!);
        var concreteType = typeof(IEventHandler<>).MakeGenericType(eventType!);
        await (Task)concreteType.GetMethod("Handle")!.Invoke(handler, [@event])!;
      }
    }
  }
}

using FINN.SHAREDKERNEL;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace FINN.CORE;

public class RabbitMqBroker : IBroker
{
    private readonly IModel _channel;
    private readonly string _exchange;
    private readonly List<Action<ReadOnlyMemory<byte>>> _handlers = new();
    private readonly ILogger<RabbitMqBroker> _logger;

    public RabbitMqBroker(ILogger<RabbitMqBroker> logger, IConfiguration configuration)
    {
        _logger = logger;
        _exchange = configuration["RabbitMQ:Exchange"];

        var factory = new ConnectionFactory
        {
            Uri = new Uri(configuration["RabbitMQ:Uri"]),
            ClientProvidedName = configuration["RabbitMQ:ClientProvidedName"]
        };

        var connection = factory.CreateConnection();

        // initialize exchange
        _channel = connection.CreateModel();
        _channel.ExchangeDeclare(_exchange, ExchangeType.Direct);
    }

    public void Send(string routingKey, ReadOnlyMemory<byte> body)
    {
        _channel.BasicPublish(_exchange, routingKey,
            null,
            body);
    }

    public void RegisterHandler(string routingKey, Action<ReadOnlyMemory<byte>> handler)
    {
        _handlers.Add(handler);

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (sender, args) => { handler(args.Body.ToArray()); };

        var queue = _channel.QueueDeclare().QueueName;
        _channel.QueueBind(queue, _exchange, routingKey);
        _logger.LogInformation(
            "[{QueueName}][{RoutingKey}]{Status}", queue, routingKey, "created");

        _channel.BasicConsume(queue, true, consumer);
        _logger.LogInformation(
            "[{QueueName}][{RoutingKey}]{Status}", queue, routingKey, "started");
    }
}
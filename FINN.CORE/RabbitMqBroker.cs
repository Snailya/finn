using System.Collections.Concurrent;
using System.Text;
using FINN.SHAREDKERNEL.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace FINN.CORE;

public class RabbitMqBroker : IBroker
{
    private readonly ConcurrentDictionary<string, TaskCompletionSource<string>> _callbackMapper = new();
    private readonly IModel _channel;
    private readonly string _exchange;
    private readonly ILogger<RabbitMqBroker> _logger;
    private readonly string _replyQueueName;

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

        // initialize a response queue
        _replyQueueName = _channel.QueueDeclare().QueueName;
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            if (!_callbackMapper.TryRemove(ea.BasicProperties.CorrelationId, out var tcs))
                return;
            var body = ea.Body.ToArray();
            var response = Encoding.UTF8.GetString(body);
            tcs.TrySetResult(response);
        };
        _channel.BasicConsume(consumer: consumer, queue: _replyQueueName, autoAck: true);
        _logger.LogInformation(
            "Reply Queue [{ReplyQueueName}] {Status}", _replyQueueName, "created");
    }

    public void Send(string routingKey, string message)
    {
        _channel.BasicPublish(_exchange, routingKey, null, Encoding.UTF8.GetBytes(message));
    }

    public Task<string> SendAsync(string routingKey, string message,
        CancellationToken cancellationToken = default)
    {
        var props = _channel.CreateBasicProperties();
        var correlationId = Guid.NewGuid().ToString();
        props.CorrelationId = correlationId;
        props.ReplyTo = _replyQueueName;
        var tcs = new TaskCompletionSource<string>();
        _callbackMapper.TryAdd(correlationId, tcs);

        _channel.BasicPublish(_exchange, routingKey, props, Encoding.UTF8.GetBytes(message));

        cancellationToken.Register(() => _callbackMapper.TryRemove(correlationId, out var tmp));
        return tcs.Task;
    }

    public void Reply(string queue, string correlationId, string message)
    {
        var replyProps = _channel.CreateBasicProperties();
        replyProps.CorrelationId = correlationId;
        _channel.BasicPublish("", queue, replyProps, Encoding.UTF8.GetBytes(message));
    }

    public void RegisterHandler(string routingKey, Action<string, string, string> handler)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            var props = ea.BasicProperties;
            handler(props.ReplyTo, props.CorrelationId, Encoding.UTF8.GetString(ea.Body.ToArray()));
        };

        var queue = _channel.QueueDeclare(routingKey).QueueName;
        _channel.QueueBind(queue, _exchange, routingKey);
        _logger.LogInformation(
            "[{QueueName}][{RoutingKey}] {Status}", queue, routingKey, "created");

        _channel.BasicConsume(queue, false, consumer);
        _logger.LogInformation(
            "[{QueueName}][{RoutingKey}] {Status}", queue, routingKey, "started");
    }
}
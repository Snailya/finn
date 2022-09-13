using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using FINN.CORE.Interfaces;
using FINN.CORE.Models;
using FINN.SHAREDKERNEL.Constants;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace FINN.BROKER.RABBITMQ;

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
        _replyQueueName = _channel.QueueDeclare($"{factory.ClientProvidedName}-reply").QueueName;
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            var props = ea.BasicProperties;
            _logger.LogInformation(
                "[{DateTime}] Reply received. Correlation Id: {CorrelationId}", DateTime.Now,
                props.CorrelationId);

            if (!_callbackMapper.TryRemove(props.CorrelationId, out var tcs))
                return;
            var response = Encoding.UTF8.GetString(ea.Body.ToArray());
            _logger.LogInformation(
                "[{DateTime}] Reply decoded. Content: {Content}", DateTime.Now, response);
            tcs.TrySetResult(response);
        };
        _channel.BasicConsume(consumer: consumer, queue: _replyQueueName, autoAck: true);

        _logger.LogInformation(
            "[{DateTime}] Reply queue created. Queue: {QueueName}", DateTime.Now, _replyQueueName);
    }

    /// <inheritdoc />
    public void Send(string routingKey, string message)
    {
        _channel.BasicPublish(_exchange, routingKey, null, Encoding.UTF8.GetBytes(message));

        _logger.LogInformation("[{DateTime}] Message sent. Receiver: {Receiver}. Content: {Content}", DateTime.Now,
            routingKey, message);
    }

    /// <inheritdoc />
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

        _logger.LogInformation(
            "[{DateTime}] Message sent and waiting for reply. Receiver: {Receiver}. Content: {Content}", DateTime.Now,
            routingKey, message);

        cancellationToken.Register(() => _callbackMapper.TryRemove(correlationId, out var tmp));
        return tcs.Task;
    }

    /// <inheritdoc />
    public void Reply(string replyTo, string correlationId, string message)
    {
        var replyProps = _channel.CreateBasicProperties();
        replyProps.CorrelationId = correlationId;
        _channel.BasicPublish("", replyTo, replyProps, Encoding.UTF8.GetBytes(message));

        _logger.LogInformation(
            "[{DateTime}] Message replied. Receiver: {Receiver}. Correlation Id: {CorrelationId}. Content: {Content}",
            DateTime.Now,
            replyTo, correlationId, message);
    }

    /// <inheritdoc />
    public void RegisterHandler(string routingKey, Action<string, string, string> handler)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            // assure received
            _channel.BasicAck(ea.DeliveryTag, false);

            var props = ea.BasicProperties;
            var message = Encoding.UTF8.GetString(ea.Body.ToArray());

            _logger.LogInformation(
                "[{DateTime}] Message received. Reply to: {ReplyTo}. Content: {Content}", DateTime.Now,
                props.ReplyTo, message);

            try
            {
                // handle
                handler(props.ReplyTo, props.CorrelationId, message);
            }
            catch (JsonException e)
            {
                Reply(props.ReplyTo, props.CorrelationId,
                    new Response(e.Message, ErrorCodes.DeserializeFailure).ToJson());

                _logger.LogError("[{DateTime}] Error occured. Type: {Type} Trace: {Trace}", DateTime.Now,
                    nameof(JsonException), e.StackTrace);
            }
            catch (ArgumentException e)
            {
                Reply(props.ReplyTo, props.CorrelationId,
                    new Response(e.Message, ErrorCodes.InvalidArgument).ToJson());
                _logger.LogError("[{DateTime}] Error occured. Type: {Type} Trace: {Trace}", DateTime.Now,
                    nameof(ArgumentException), e.StackTrace);
            }
            catch (Exception e)
            {
                Reply(props.ReplyTo, props.CorrelationId, new Response(e.Message, ErrorCodes.Unknown).ToJson());
                _logger.LogError("[{DateTime}] Error occured. Type: {Type} Trace: {Trace}", DateTime.Now,
                    nameof(ArgumentException), e.StackTrace);
            }
        };

        var queue = _channel.QueueDeclare(routingKey).QueueName;
        _channel.QueueBind(queue, _exchange, routingKey);
        _channel.BasicConsume(queue, false, consumer);

        _logger.LogInformation(
            "[{DateTime}] Handler registered. Route: {Route}, Queue: {QueueName}", DateTime.Now, routingKey, queue);
    }
}
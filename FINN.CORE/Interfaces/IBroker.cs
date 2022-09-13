namespace FINN.CORE.Interfaces;

public interface IBroker
{
    /// <summary>
    ///     Send message to specified routing. This method is used for Publish/Subscribe pattern.
    /// </summary>
    void Send(string routingKey, string message);

    /// <summary>
    ///     Send message to specified routing and get the response. This method is used for Request/Reply pattern.
    /// </summary>
    Task<string> SendAsync(string routingKey, string message, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Send reply message to the reply queue.
    /// </summary>
    void Reply(string replyTo, string correlationId, string message);

    /// <summary>
    ///     Register handler for subscription.
    /// </summary>
    void RegisterHandler(string routingKey, Action<string, string, string> handler);
}
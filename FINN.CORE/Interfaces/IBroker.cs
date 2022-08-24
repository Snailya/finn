namespace FINN.CORE.Interfaces;

public interface IBroker
{
    /// <summary>
    ///     Send message to specified routing. This method is used for Publish/Subscribe pattern.
    /// </summary>
    /// <param name="routingKey"></param>
    /// <param name="message"></param>
    void Send(string routingKey, string message);

    /// <summary>
    ///     Send message to specified routing and get the response. This method is used for Request/Reply pattern.
    /// </summary>
    /// <param name="routingKey"></param>
    /// <param name="message"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<string> SendAsync(string routingKey, string message, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Send reply message to the reply queue.
    /// </summary>
    /// <param name="queue"></param>
    /// <param name="correlationId"></param>
    /// <param name="message"></param>
    void Reply(string queue, string correlationId, string message);

    /// <summary>
    ///     Register handler for subscription.
    /// </summary>
    /// <param name="routingKey"></param>
    /// <param name="handler"></param>
    void RegisterHandler(string routingKey, Action<string, string, string> handler);
}
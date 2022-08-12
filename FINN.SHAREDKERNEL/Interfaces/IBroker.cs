namespace FINN.SHAREDKERNEL;

public interface IBroker
{
    void Send(string routingKey, ReadOnlyMemory<byte> body);
    void RegisterHandler(string routingKey, Action<ReadOnlyMemory<byte>> handler);
}
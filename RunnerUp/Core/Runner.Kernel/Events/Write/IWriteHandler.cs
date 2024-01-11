
namespace Runner.Kernel.Events.Write
{
    public interface IWriteHandler<TRequest> where TRequest : IWrite
    {
        Task Handler(EventProcess process, TRequest request, CancellationToken cancellationToken);
    }
}

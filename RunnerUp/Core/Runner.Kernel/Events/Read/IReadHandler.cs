
namespace Runner.Kernel.Events.Read
{
    public interface IReadHandler<TRequest, TResult> where TRequest : IRead<TResult>
    {
        Task<TResult> Handler(EventProcess process, TRequest request, CancellationToken cancellationToken);
    }
}

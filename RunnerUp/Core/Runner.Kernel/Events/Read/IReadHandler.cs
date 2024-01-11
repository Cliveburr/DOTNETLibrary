
namespace Runner.Kernel.Events.Read
{
    public interface IReadHandler<TRequest, TResult> where TRequest : IRead<TResult>
    {
        TResult Handler(TRequest request, CancellationToken cancellationToken);
    }
}

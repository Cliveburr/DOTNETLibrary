
namespace Runner.Kernel.Events.Command
{
    public interface ICommandResultHandler<TRequest, TResult> where TRequest : ICommandResult<TResult>
    {
        Task<TResult> Handler(EventProcess process, TRequest request, CancellationToken cancellationToken);
    }
}

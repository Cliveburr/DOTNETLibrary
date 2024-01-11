
namespace Runner.Kernel.Events.Command
{
    public interface ICommandHandler<TRequest> where TRequest : ICommand
    {
        Task Handler(EventProcess process, TRequest request, CancellationToken cancellationToken);
    }
}

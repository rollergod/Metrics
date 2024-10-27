using System.Windows.Input;

namespace Metrics.Abstractions.Commands;

public interface ICommand<TResult>
{
}

public interface ICommandHandler<in TCommand, TResult> where TCommand : ICommand<TResult>
{
    Task<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken);
}

public interface ICommandDispatcher
{
    Task<TResult> HandleAsync<TCommand,TResult>(TCommand command, CancellationToken cancellationToken) where TCommand : ICommand<TResult>;
}

public class CommandDispatcher(IServiceProvider serviceProvider) : ICommandDispatcher
{
    public async Task<TResult> HandleAsync<TCommand, TResult>(TCommand command, CancellationToken cancellationToken) where TCommand : ICommand<TResult>
    {
        var handler = (ICommandHandler<TCommand,TResult>)serviceProvider.GetService(typeof(ICommandHandler<TCommand,TResult>)); 
        return await handler.HandleAsync(command, cancellationToken);   
    }
}
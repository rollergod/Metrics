namespace Metrics.Abstractions.Query;

public interface IQuery<TResult>
{
}

public interface IQueryHandler<in TQuery, TResult> where TQuery : IQuery<TResult>
{
    Task<TResult> ExecuteAsync(TQuery query, CancellationToken cancellationToken);
}

public interface IQueryDispatcher
{
    Task<TResult> HandleAsync<TQuery,TResult>(TQuery query, CancellationToken cancellationToken) where TQuery : IQuery<TResult>;
}

public class QueryDispatcher(IServiceProvider serviceProvider) : IQueryDispatcher
{
    public async Task<TResult> HandleAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken) where TQuery : IQuery<TResult>
    {
        var handler = (IQueryHandler<TQuery,TResult>)serviceProvider.GetService(typeof(IQueryHandler<TQuery,TResult>));
        return await handler.ExecuteAsync(query, cancellationToken);
    }
}
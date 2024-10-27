using Metrics.Abstractions.Query;

namespace Metrics.CQRS.Queries.People;

public class Query : IQuery<ViewModel>
{
    public int Id { get; set; }
}
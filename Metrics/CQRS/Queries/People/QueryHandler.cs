using Metrics.Abstractions.Query;
using Microsoft.EntityFrameworkCore;

namespace Metrics.CQRS.Queries.People;

public class QueryHandler(AppDbContext context) : IQueryHandler<Query,ViewModel>
{
    public async Task<ViewModel> ExecuteAsync(Query query, CancellationToken cancellationToken)
    {
        //dapper suda nado
        var res = await context.People.FirstOrDefaultAsync(x => x.Id == query.Id, cancellationToken);
        return new ViewModel{Id = res.Id, Name = res.Name};
    }
}
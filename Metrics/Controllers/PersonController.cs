using System.Diagnostics.Metrics;
using Metrics.Abstractions.Query;
using Metrics.Domain;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Person = Metrics.CQRS.Queries.People;

namespace Metrics.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonController(AppDbContext context, IMeterFactory meterFactory) : ControllerBase
    {
        [HttpPost]
        public async Task<int> CreatePerson(string name)
        {
            context.People.Add(new Domain.Person
            {
                Name = name
            });
            var res = await context.SaveChangesAsync();

            return res;
        }

        [HttpGet("{id:int}")]
        public async Task<Person.ViewModel> GetById(
            int id, 
            CancellationToken cancellationToken,
            [FromServices] IQueryDispatcher queryDispatcher)
        {
            var query = new Person.Query{Id = id};
            return await queryDispatcher.HandleAsync<Person.Query, Person.ViewModel>(query, cancellationToken);
        }

        [HttpGet]
        public int Test()
        {
            //можно добавлять каунтеры на методы 
            var meter = meterFactory.Create(new MeterOptions("PersonController"));
            var instrument = meter.CreateCounter<int>("test_counter");
            instrument.Add(1);
            return 1;
        }
    }
}

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Metrics.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonController : ControllerBase
    {
        [HttpPost]
        public async Task<int> CreatePerson(AppDbContext context, string name)
        {

            var entry = context.People.Add(new Domain.Person
            {
                Name = name
            });
            await context.SaveChangesAsync();

            return entry.Entity.Id;
        }
    }
}

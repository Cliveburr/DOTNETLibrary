using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DependencyResolver.WebTests.Subjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

namespace DependencyResolver.WebTests.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get([FromServices] IServiceProvider provider)
        {
            var subject0 = provider.GetService(typeof(SubjectTest)) as SubjectTest;

            var subject1 = provider.GetService(typeof(SubjectTest)) as SubjectTest;

            return new string[] { "value1", "value2", "subject0 id", subject0.Id.ToString(), "subject1 id", subject1.Id.ToString() };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

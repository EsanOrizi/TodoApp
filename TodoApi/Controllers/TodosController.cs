using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using TodoLibrary.DataAccess;
using TodoLibrary.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodosController : ControllerBase
    {
        private readonly ITodoData data;

        public TodosController(ITodoData data)
        {
            this.data = data;
            
        }

        private int GetUserId()
        {

            var userIdText = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdText);
        }

         

        // GET: api/Todos
        [HttpGet]
        public async Task<ActionResult<List<TodoModel>>> Get()
        {
            var userIdText = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var output = await data.GetAllAssigned(GetUserId());        
            return Ok(output);
        }

        // GET api/Todos/5
        [HttpGet("{todoId}")]
        public async Task<ActionResult<TodoModel>> Get(int todoId)
        {
            var output = await data.GetOneAssigned(GetUserId(), todoId);

            return Ok(output);
        }

        // POST api/Todos
        [HttpPost]
        public async Task<ActionResult<TodoModel>> Post([FromBody] string task)
        {
            var output = await data.Create(GetUserId(), task);
            return Ok(output);  
        }

        // PUT api/Todos/5
        [HttpPut("{todoId}")]
        public async Task<ActionResult> Put(int todoId, [FromBody] string task)
        {
            await data.UpdateTask(GetUserId(), todoId, task);
            return Ok();
        }

        [HttpPut("{todoId}/Complete")]
        public async Task<IActionResult> Complete(int todoId)
        {
            await data.CompleteTodo(GetUserId(), todoId);
            return Ok();

        }

        // DELETE api/Todos/5
        [HttpDelete("{todoId}")]
        public async Task<IActionResult> Delete(int todoId)
        {
            await data.Delete(GetUserId(), todoId);
            return Ok();

        }
    }
}

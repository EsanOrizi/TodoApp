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
        private readonly ILogger<TodosController> logger;

        public TodosController(ITodoData data, ILogger<TodosController> logger)
        {
            this.data = data;
            this.logger = logger;
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

            logger.LogInformation("GET: api/Todos");
            try
            {
                var output = await data.GetAllAssigned(GetUserId());
                return Ok(output);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GET call to api todos failed.");
                return BadRequest();
            }            
        }

        // GET api/Todos/5
        [HttpGet("{todoId}")]
        public async Task<ActionResult<TodoModel>> Get(int todoId)
        {
            logger.LogInformation("GET: api/Todos/{todoId}", todoId);
            try
            {
                var output = await data.GetOneAssigned(GetUserId(), todoId);

                return Ok(output);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GET call to {ApiPath} todos failes. The Id was {todoId}", $"api/Todos/Id", todoId);
                return BadRequest();
            }
        }

        // POST api/Todos
        [HttpPost]
        public async Task<ActionResult<TodoModel>> Post([FromBody] string task)
        {
            logger.LogInformation("POST: api/Todos/{task}", task);
            try
            {
                var output = await data.Create(GetUserId(), task);
                return Ok(output);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "POST call to api todos failed.");
                return BadRequest();
            }  
        }

        // PUT api/Todos/5
        [HttpPut("{todoId}")]
        public async Task<ActionResult> Put(int todoId, [FromBody] string task)
        {
            logger.LogInformation("PUT: api/Todos/{task}", task);
            try
            {
                await data.UpdateTask(GetUserId(), todoId, task);
                return Ok();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "PUT call to api todos failed.");
                return BadRequest();
            }
        }

        [HttpPut("{todoId}/Complete")]
        public async Task<IActionResult> Complete(int todoId)
        {
            logger.LogInformation("PUT: api/Todos/{todoID}/Complete", todoId);
            try
            {
                await data.CompleteTodo(GetUserId(), todoId);
                return Ok();

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Complete call to api todos failed.");
                return BadRequest();
            }
        }

        // DELETE api/Todos/5
        [HttpDelete("{todoId}")]
        public async Task<IActionResult> Delete(int todoId)
        {

            logger.LogInformation("PUT: api/Todos/{todoId}", todoId);
            try
            {
                await data.Delete(GetUserId(), todoId);
                return Ok();
            }
            catch (Exception ex)
            {

                logger.LogError(ex, "Delete call to api todos failed.");
                return BadRequest();
            }

        }
    }
}

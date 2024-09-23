using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using TodoListApi.Data;
using TodoListApi.Models;
using Microsoft.Extensions.Logging;
// using System.Diagnostics;

namespace TodoListApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TodosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger _logger;

        [ActivatorUtilitiesConstructor] // fix error Multiple constructors accepting all given...
        public TodosController(ILogger<TodosController> logger, AppDbContext context, UserManager<IdentityUser> userManager)
        {
             _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Todo>>> GetTodos(string? lastId = null, int pageSize = 10)
        {
            _logger.LogInformation("Call api get todos {DT}", 
            DateTime.UtcNow.ToLongTimeString());

            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized("User not authenticated.");
            }

            // Convert lastId to an integer for comparison
            int parsedLastId = lastId != null ? int.Parse(lastId) : 0;

            // var stopwatch = new Stopwatch();
            // stopwatch.Start();

            // var todos = await _context.Todos
            //     .Where(t => t.UserId == userId && (lastId == null || t.Id > parsedLastId))
            //     .Take(pageSize)
            //     .OrderBy(t => t.Id) 
            //     .ToListAsync();
            
            // Use indexed for pagination
            var todos = await _context.Todos
                .Where(t => t.UserId == userId && t.Id > parsedLastId)
                .OrderBy(t => t.Title)                           
                .Take(pageSize)                                  
                .ToListAsync();
            
            // stopwatch.Stop();
            // Console.WriteLine($"Query used time: {stopwatch.ElapsedMilliseconds} ms");

            var todoDtos = todos.Select(t => new TodoDTO
            {
                Title = t.Title,
                IsCompleted = t.IsComplete
            }).ToList();

            var response = new
            {
                PageSize = pageSize,
                Todos = todoDtos,
                NextLastId = todos.Count > 0 ? todos.Last().Id.ToString() : null
            };

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTodoById(int id)
        {

            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized("User not authenticated.");
            }

            var todo = await _context.Todos
        .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (todo == null)
            {
                return NotFound("Todo not found or you do not have access to it.");
            }

            var todoDto = new TodoDTO
            {
                Title = todo.Title,
                IsCompleted = todo.IsComplete
            };

            return Ok(todoDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTodo([FromBody] TodoDTO todoDto)
        {

            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized("User not authenticated.");
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return Unauthorized("User not authenticated.");
            }

            var todo = new Todo
            {
                Title = todoDto.Title,
                IsComplete = todoDto.IsCompleted,
                UserId = userId,
            };

            _context.Todos.Add(todo);
            await _context.SaveChangesAsync();

            var responseDto = new TodoDTO
            {
                Title = todo.Title,
                IsCompleted = todo.IsComplete
            };

            return CreatedAtAction(nameof(GetTodoById), new { id = todo.Id }, responseDto);

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodo(int id, [FromBody] Todo todo)
        {

            if (id != todo.Id)
            {
                return BadRequest();
            }

            var userId = _userManager.GetUserId(User);
            if (userId == null || userId != todo.UserId)
            {
                return Unauthorized(); // Handle unauthorized access
            }

            _context.Entry(todo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodo(int id)
        {
            var todo = await _context.Todos.FindAsync(id);
            if (todo == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            if (userId == null || userId != todo.UserId)
            {
                return Unauthorized();
            }

            _context.Todos.Remove(todo);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        private bool TodoExists(int id)
        {
            return _context.Todos.Any(e => e.Id == id);
        }
    }
}
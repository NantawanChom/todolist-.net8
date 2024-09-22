using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using TodoListApi.Data;
using TodoListApi.Models;
using System.Security.Claims;

namespace TodoListApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TodosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public TodosController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Todo>>> GetTodos()
        {
            // Get the UserId of the logged-in user
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized("User not authenticated.");
            }

            // Query the Todos for the logged-in user
            var todos = await _context.Todos
                .Where(t => t.UserId == userId)
                .ToListAsync();

            // Optionally, you can map to a DTO if you want to exclude sensitive information
            var todoDtos = todos.Select(t => new TodoDTO
            {
                Title = t.Title,
                IsCompleted = t.IsComplete
            }).ToList();

            return Ok(todoDtos);
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

            // Set UserId to the currently logged-in user's Id
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized("User not authenticated.");
            }

            // Fetch the User object using the UserId
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return Unauthorized("User not authenticated.");
            }

            // Map the DTO to the actual Todo entity
            var todo = new Todo
            {
                Title = todoDto.Title,
                IsComplete = todoDto.IsCompleted,
                UserId = userId,
            };

            _context.Todos.Add(todo);
            await _context.SaveChangesAsync();

            // Create the response DTO
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
                return Unauthorized(); // Handle unauthorized access
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
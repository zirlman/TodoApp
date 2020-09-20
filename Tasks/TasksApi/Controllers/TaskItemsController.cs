using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TasksApi.Model;
using TasksApi.Model.Context;

namespace TasksApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TaskItemsController : ControllerBase
    {
        private readonly UserTaskContext _context;
        private readonly ILogger<TaskItemsController> _logger;

        public TaskItemsController(UserTaskContext context, ILogger<TaskItemsController> logger)
        {
            _logger = logger;
            _context = context;
            // Create the database if not exists
            _context.Database.EnsureCreated();
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<TaskItem>> GetTaskItem(long id)
        {
            var taskItem = await _context.TaskItem.FindAsync(id);

            if (taskItem == null)
            {
                return NotFound();
            }

            return taskItem;
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchTaskItem(long id, [FromBody] JsonPatchDocument<TaskItem> patchDoc)
        {
            if (patchDoc != null)
            {
                var todoItem = await _context.TaskItem.FindAsync(id);

                if (todoItem != null)
                {
                    patchDoc.ApplyTo(todoItem, ModelState);

                    if (!ModelState.IsValid)
                    {
                        return BadRequest(ModelState);
                    }

                    try
                    {
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!Exists(id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            _logger.LogInformation($"UNABLE TO PATCH TASK ITEM WITH ID: {id}");
                            throw;
                        }
                    }
                }
                else
                {
                    return NotFound(new { message = $"Unable to find TodoItem with id {id}" });
                }

                return new ObjectResult(todoItem);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpPost]
        public async Task<ActionResult<TaskItem>> PostTaskItem(TaskItem taskItem)
        {
            int userId;
            var claimValue = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            if (int.TryParse(claimValue, out userId) && _context.User.Any(u => u.UserId == userId))
            {
                taskItem.UserId = userId;
                _context.TaskItem.Add(taskItem);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"NEW TASK: {taskItem.TaskItemId}");
                return CreatedAtAction("GetTaskItem", new { id = taskItem.TaskItemId }, taskItem);
            }
            else
            {
                return BadRequest(new { message = "Invalid claim" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<TaskItem>> DeleteTaskItem(long id)
        {
            var taskItem = await _context.TaskItem.FindAsync(id);
            if (taskItem == null)
            {
                return NotFound();
            }

            _context.TaskItem.Remove(taskItem);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"DELETE TASK ID: {id}");
            return taskItem;
        }

        private bool Exists(long id)
        {
            return _context.TaskItem.Any(e => e.TaskItemId == id);
        }
    }
}

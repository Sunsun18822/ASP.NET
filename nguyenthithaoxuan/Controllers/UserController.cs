using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nguyenthithaoxuan.Data;
using nguyenthithaoxuan.Models;

namespace nguyenthithaoxuan.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/user
        [HttpGet]
        public ActionResult<IEnumerable<User>> Get()
        {
            var users = _context.Users.Where(u => !u.IsDeleted).ToList();
            return Ok(users);
        }

        // GET: api/user/{id}
        [HttpGet("{id}")]
        public ActionResult<User> Get(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id && !u.IsDeleted);
            if (user == null)
                return NotFound();
            return Ok(user);
        }

        // POST: api/user
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] User user)
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
            user.CreatedAt = DateTime.UtcNow;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = user.Id }, user);
        }

        // PUT: api/user/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] User updatedUser)
        {
            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null)
                return NotFound();

            existingUser.FullName = updatedUser.FullName;
            existingUser.Email = updatedUser.Email;
            existingUser.Phone = updatedUser.Phone;
            existingUser.Address = updatedUser.Address;
            existingUser.Role = updatedUser.Role;
            existingUser.Status = updatedUser.Status;
            existingUser.IsDeleted = updatedUser.IsDeleted; // 👈 Cho phép thay đổi trạng thái ẩn/hiện
            existingUser.UpdatedAt = DateTime.UtcNow;
            existingUser.UpdatedBy = updatedUser.UpdatedBy;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/user/{id} - Soft delete
        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null || user.IsDeleted)
                return NotFound();

            user.IsDeleted = true;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // GET: api/user/trash - Get all soft-deleted users
        [HttpGet("trashed")]
        public IActionResult GetTrashedUsers()
        {
            var deletedUsers = _context.Users
                .Where(u => u.IsDeleted)
                .ToList();
            return Ok(deletedUsers);
        }

        // PUT: api/user/restore/{id} - Restore soft-deleted user
        [HttpPut("restore/{id}")]
        public async Task<IActionResult> Restore(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null || !user.IsDeleted)
                return NotFound();

            user.IsDeleted = false;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/user/permanent/{id} - Permanent delete
        [HttpDelete("permanent/{id}")]
        public async Task<IActionResult> PermanentDelete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null || !user.IsDeleted)
                return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}

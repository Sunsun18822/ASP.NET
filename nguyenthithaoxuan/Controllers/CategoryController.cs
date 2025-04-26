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
    public class CategoryController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/category
        // Chỉ trả về các category chưa bị soft-delete
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetAll()
        {
            var categories = await _context.Categories
                .Where(c => !c.IsDeleted)
                .Include(c => c.Products)
                .ToListAsync();
            return Ok(categories);
        }

        // GET: api/category/trashed
        // Trả về các category đã soft-delete
        [HttpGet("trashed")]
        public async Task<ActionResult<IEnumerable<Category>>> GetTrashed()
        {
            var trashed = await _context.Categories
                .Where(c => c.IsDeleted)
                .Include(c => c.Products)
                .ToListAsync();
            return Ok(trashed);
        }

        // GET: api/category/{id}
        // Nếu item đã soft-delete vẫn có thể lấy chi tiết
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetById(int id)
        {
            var category = await _context.Categories
                .IgnoreQueryFilters()    // nếu có filter global
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
                return NotFound();

            return Ok(category);
        }

        // POST: api/category
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Category category)
        {
            category.CreatedAt = DateTime.UtcNow;
            category.IsDeleted = false;      // đảm bảo mặc định

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById),
                                   new { id = category.Id },
                                   category);
        }

        // PUT: api/category/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Category updatedCategory)
        {
            var existing = await _context.Categories
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(c => c.Id == id);
            if (existing == null)
                return NotFound();

            existing.Name = updatedCategory.Name;
            existing.Description = updatedCategory.Description;
            existing.Status = updatedCategory.Status;
            existing.UpdatedAt = DateTime.UtcNow;
            existing.UpdatedBy = updatedCategory.UpdatedBy;
            // giữ nguyên existing.IsDeleted

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/category/{id}
        // API để xử lý soft-delete
        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var category = await _context.Categories
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(c => c.Id == id);
            if (category == null)
                return NotFound();

            category.IsDeleted = true;  // Đánh dấu là đã xóa mềm
            await _context.SaveChangesAsync();
            return NoContent();  // Trả về mã 204 khi thành công
        }

        // DELETE: api/category/{id}/destroy
        // Hard-delete (xóa vĩnh viễn)
        [HttpDelete("{id}/destroy")]
        public async Task<IActionResult> HardDelete(int id)
        {
            var category = await _context.Categories
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(c => c.Id == id);
            if (category == null)
                return NotFound();

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // PUT: api/category/{id}/restore
        // Phục hồi từ thùng rác
        [HttpPut("{id}/restore")]
        public async Task<IActionResult> Restore(int id)
        {
            var category = await _context.Categories
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(c => c.Id == id);
            if (category == null)
                return NotFound();

            category.IsDeleted = false;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}

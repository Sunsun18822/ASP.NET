using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nguyenthithaoxuan.Data;
using nguyenthithaoxuan.Models;

namespace nguyenthithaoxuan.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PaymentController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Payment
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var payments = await _context.Payments.Include(p => p.Order).ToListAsync();
            return Ok(payments);
        }

        // GET: api/Payment/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var payment = await _context.Payments.Include(p => p.Order).FirstOrDefaultAsync(p => p.Id == id);
            if (payment == null) return NotFound();
            return Ok(payment);
        }

        // POST: api/Payment
        [HttpPost]
        public async Task<IActionResult> Create(Payment payment)
        {
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = payment.Id }, payment);
        }

        // PUT: api/Payment/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Payment payment)
        {
            if (id != payment.Id) return BadRequest();

            var existing = await _context.Payments.FindAsync(id);
            if (existing == null) return NotFound();

            existing.OrderId = payment.OrderId;
            existing.PaymentMethod = payment.PaymentMethod;
            existing.PaymentStatus = payment.PaymentStatus;
            existing.PaymentDate = payment.PaymentDate;
            existing.UpdatedAt = DateTime.UtcNow;
            existing.UpdatedBy = payment.UpdatedBy;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Payment/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null) return NotFound();

            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}

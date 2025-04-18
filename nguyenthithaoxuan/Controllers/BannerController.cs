﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nguyenthithaoxuan.Data;
using nguyenthithaoxuan.Models;

namespace nguyenthithaoxuan.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BannerController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BannerController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/banner
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Banner>>> Get()
        {
            var banners = await _context.Banners.ToListAsync();
            return Ok(banners);
        }

        // GET: api/banner/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Banner>> Get(int id)
        {
            var banner = await _context.Banners.FindAsync(id);
            if (banner == null)
                return NotFound();
            return Ok(banner);
        }

        // POST: api/banner
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Banner banner)
        {
            banner.CreatedAt = DateTime.UtcNow;
            _context.Banners.Add(banner);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = banner.Id }, banner);
        }

        // PUT: api/banner/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Banner updatedBanner)
        {
            var existingBanner = await _context.Banners.FindAsync(id);
            if (existingBanner == null)
                return NotFound();

            existingBanner.ImageUrl = updatedBanner.ImageUrl;
            existingBanner.Title = updatedBanner.Title;
            existingBanner.Description = updatedBanner.Description;
            existingBanner.Status = updatedBanner.Status;
            existingBanner.UpdatedAt = DateTime.UtcNow;
            existingBanner.UpdatedBy = updatedBanner.UpdatedBy;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/banner/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var banner = await _context.Banners.FindAsync(id);
            if (banner == null)
                return NotFound();

            _context.Banners.Remove(banner);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
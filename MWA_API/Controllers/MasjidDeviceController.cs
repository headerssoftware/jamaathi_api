using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MWA_API.Data;
using MWA_API.Models;

namespace MWA_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MasjidDeviceController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public MasjidDeviceController(ApplicationDbContext context)
        {
            this._context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<MasjidDevice>>> GetAll()
        {
            return await _context.masjidDevices.AsNoTracking().ToListAsync();
        }

        [HttpGet("{id:int}", Name = "getMasjidDevice")]
        public async Task<ActionResult<MasjidDevice>> GetByIds(int id)
        {
            var result = await _context.masjidDevices.FirstOrDefaultAsync(x => x.masjidDeviceId == id);
            if (result != null)
                return result;
            else
                return NotFound();

        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] MasjidDevice curr)
        {
            try
            {
                _context.Add(curr);
                await _context.SaveChangesAsync();
                return new CreatedAtRouteResult("getMasjidDevice", new { Id = curr.masjidDeviceId }, curr);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        [Route("{id:int}")]

        [HttpPut]
        public async Task<ActionResult> Put(int id, [FromBody] MasjidDevice curr)
        {
            if (id != curr.masjidDeviceId)
            {
                return BadRequest();
            }

            try
            {
                curr.masjidDeviceId = id;
                _context.Entry(curr).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [Route("{id:int}")]
        [HttpDelete]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var exists = await _context.masjidDevices.AnyAsync(x => x.masjidDeviceId == id);
                if (!exists)
                {
                    return NotFound();
                }
                _context.Remove(new MasjidDevice() { masjidDeviceId = id });
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }


       
    }





}

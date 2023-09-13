using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MWA_API.Data;
using MWA_API.Models;

namespace MWA_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WaqthMasterController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public WaqthMasterController(ApplicationDbContext context)
        {
            this._context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<WaqthMaster>>> GetAll()
        {
            return await _context.waqthMasters.AsNoTracking().ToListAsync();
        }

        [HttpGet("{id:int}", Name = "getWaqthMaster")]
        public async Task<ActionResult<WaqthMaster>> GetByIds(int id)
        {
            var result = await _context.waqthMasters.FirstOrDefaultAsync(x => x.waqthId == id);
            if (result != null)
                return result;
            else
                return NotFound();

        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] WaqthMaster curr)
        {
            try
            {
                _context.Add(curr);
                await _context.SaveChangesAsync();
                return new CreatedAtRouteResult("getWaqthMaster", new { Id = curr.waqthId }, curr);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        [Route("{id:int}")]

        [HttpPut]
        public async Task<ActionResult> Put(int id, [FromBody] WaqthMaster curr)
        {
            if (id != curr.waqthId)
            {
                return BadRequest();
            }

            try
            {
                curr.waqthId = id;
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
                var exists = await _context.waqthMasters.AnyAsync(x => x.waqthId == id);
                if (!exists)
                {
                    return NotFound();
                }
                _context.Remove(new WaqthMaster() { waqthId = id });
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

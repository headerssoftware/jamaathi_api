using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MWA_API.Data;
using MWA_API.Models;

namespace MWA_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OsTypeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public OsTypeController(ApplicationDbContext context)
        {
            this._context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<OsType>>> GetAll()
        {
            return await _context.osTypes.AsNoTracking().ToListAsync();
        }

        [HttpGet("{id:int}", Name = "getOsType")]
        public async Task<ActionResult<OsType>> GetById(int id)
        {
            var result = await _context.osTypes.FirstOrDefaultAsync(x => x.osTypeId == id);
            if(result != null)
                return result;
            else
                return NotFound();

        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] OsType curr)
        {
            try
            {
                _context.Add(curr);
                await _context.SaveChangesAsync();
                return new CreatedAtRouteResult("getOsType", new { Id = curr.osTypeId }, curr);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        [Route("{id:int}")]

        [HttpPut]
        public async Task<ActionResult> Put(int id, [FromBody] OsType curr)
        {
            if (id != curr.osTypeId)
            {
                return BadRequest();
            }

            try
            {
                curr.osTypeId = id;
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
                var exists = await _context.osTypes.AnyAsync(x => x.osTypeId == id);
                if (!exists)
                {
                    return NotFound();
                }
                _context.Remove(new OsType() { osTypeId = id });
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

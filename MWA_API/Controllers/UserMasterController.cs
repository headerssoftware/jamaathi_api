using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MWA_API.Data;
using MWA_API.Models;

namespace MWA_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserMasterController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public UserMasterController(ApplicationDbContext context)
        {
            this._context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<UserMaster>>> GetAll()
        {
            return await _context.userMasters.AsNoTracking().ToListAsync();
        }

        [HttpGet("{id:int}", Name = "getUserMaster")]
        public async Task<ActionResult<UserMaster>> GetById(int id)
        {
            var result = await _context.userMasters.FirstOrDefaultAsync(x => x.userId == id);
            if (result != null)
                return result;
            else
                return NotFound();

        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] UserMaster curr)
        {
            try
            {
                _context.Add(curr);
                await _context.SaveChangesAsync();
                return new CreatedAtRouteResult("getUserMaster", new { Id = curr.userId }, curr);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        [Route("{id:int}")]

        [HttpPut]
        public async Task<ActionResult> Put(int id, [FromBody] UserMaster curr)
        {
            if (id != curr.userId)
            {
                return BadRequest();
            }

            try
            {
                curr.userId = id;
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
                var exists = await _context.userMasters.AnyAsync(x => x.userId == id);
                if (!exists)
                {
                    return NotFound();
                }
                _context.Remove(new UserMaster() { userId = id });
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

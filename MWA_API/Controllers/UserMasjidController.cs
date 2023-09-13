using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MWA_API.Data;
using MWA_API.Models;

namespace MWA_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserMasjidController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public UserMasjidController(ApplicationDbContext context)
        {
            this._context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<UserMasjid>>> GetAll()
        {
            return await _context.userMasjids.AsNoTracking().ToListAsync();
        }

        [HttpGet("{id:int}", Name = "getUserMasjid")]
        public async Task<ActionResult<UserMasjid>> GetByIds(int id)
        {
            var result = await _context.userMasjids.FirstOrDefaultAsync(x => x.userMasjidId == id);
            if (result != null)
                return result;
            else
                return NotFound();

        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] UserMasjid curr)
        {
            try
            {
                _context.Add(curr);
                await _context.SaveChangesAsync();
                return new CreatedAtRouteResult("getUserMasjid", new { Id = curr.userMasjidId }, curr);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        [Route("{id:int}")]

        [HttpPut]
        public async Task<ActionResult> Put(int id, [FromBody] UserMasjid curr)
        {
            if (id != curr.userMasjidId)
            {
                return BadRequest();
            }

            try
            {
                curr.userMasjidId = id;
                _context.Entry(curr).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        //[Route("{id:int}")]
        //[HttpDelete]
        //public async Task<ActionResult> DeleteWithId(int id)
        //{
        //    try
        //    {
        //        var exists = await _context.userMasjids.AnyAsync(x => x.userMasjidId == id);
        //        if (!exists)
        //        {
        //            return NotFound();
        //        }
        //        _context.Remove(new UserMasjid() { userMasjidId = id });
        //        await _context.SaveChangesAsync();
        //        return NoContent();
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { error = ex.Message });
        //    }
        //}

        [Route("{userId:int},{masjidId:int}")]
        [HttpDelete]
        public async Task<ActionResult> Delete(int userId,int masjidId)
        {
            try
            {
                var userMasjid = await _context.userMasjids.Where(x => x.userId == userId && x.masjidId== masjidId).FirstOrDefaultAsync();
                if (userMasjid==null)
                {
                    return NotFound();
                }
                _context.Remove(userMasjid);
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

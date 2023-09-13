using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MWA_API.Data;
using MWA_API.Filters;
using MWA_API.Models;

namespace MWA_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ViewUserMasterController : ControllerBase
    {
        
        private readonly ApplicationDbContext _context;
        public ViewUserMasterController(ApplicationDbContext context)
        {
            this._context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<ViewUserMaster>>> GetAll()
        {
            return await _context.viewUserMasters.AsNoTracking().ToListAsync();
        }

        [HttpGet("filter", Name = "getViewUserMaster")]
        public async Task<ActionResult<List<ViewUserMaster>>> Filter([FromQuery] ViewUserMasterFilters filters)
        {
            try
            {
                var queryable = _context.viewUserMasters.AsQueryable();

                if (filters.userId > 0)
                {
                    queryable = queryable.Where(x => x.userId == filters.userId);
                }
                if (filters.osTypeId > 0)
                {
                    queryable = queryable.Where(x => x.osTypeId == filters.osTypeId);
                }
                if (!string.IsNullOrWhiteSpace(filters.userName))
                {
                    queryable = queryable.Where(x => x.userName == filters.userName);
                }

                if (!string.IsNullOrWhiteSpace(filters.SearchText))
                {
                    queryable = queryable.Where(x => x.userName.Contains(filters.SearchText));
                }

                var result = await queryable.ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}

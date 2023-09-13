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
    public class ViewMasjidWaqthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ViewMasjidWaqthController(ApplicationDbContext context)
        {
            this._context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<ViewMasjidWaqth>>> GetAll()
        {
            try
            {
                var result = await _context.viewMasjidWaqths.AsNoTracking().ToListAsync();
                foreach (var value in result)
                {
                    if (value.masjidImagePath != null && string.IsNullOrWhiteSpace(value.masjidImagePath) == false)
                    {
                        var fileName = Path.GetFileName(value.masjidImagePath);
                        value.masjidImageURL = $"{Request.Scheme}://{Request.Host.Value}/api/Document/getFile?name={Uri.EscapeDataString(fileName)}";
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("filter", Name = "getViewMasjidWaqth")]
        public async Task<ActionResult<List<ViewMasjidWaqth>>> Filter([FromQuery] ViewMasjidWaqthFilters filters)
        {
            try
            {
                var queryable = _context.viewMasjidWaqths.AsQueryable();

                if (filters.masjidId > 0)
                {
                    queryable = queryable.Where(x => x.masjidId == filters.masjidId);
                }
                if (filters.deviceId > 0)
                {
                    queryable = queryable.Where(x => x.deviceId == filters.deviceId);
                }
                if (filters.waqthId > 0)
                {
                    queryable = queryable.Where(x => x.waqthId == filters.waqthId);
                }
                if (!string.IsNullOrWhiteSpace(filters.waqthName))
                {
                    queryable = queryable.Where(x => x.waqthName.Contains(filters.waqthName));
                }

                if (!string.IsNullOrWhiteSpace(filters.masjidName))
                {
                    queryable = queryable.Where(x => x.masjidName.Contains(filters.masjidName));
                }

                if (!string.IsNullOrWhiteSpace(filters.masjidAddress))
                {
                    queryable = queryable.Where(x => x.masjidAddress.Contains(filters.masjidAddress));
                }

                if (!string.IsNullOrWhiteSpace(filters.masjidPincode))
                {
                    queryable = queryable.Where(x => x.masjidPincode.Contains(filters.masjidPincode));
                }

                if (!string.IsNullOrWhiteSpace(filters.masjidMadhab))
                {
                    queryable = queryable.Where(x => x.masjidMadhab.Contains(filters.masjidMadhab));
                }

                if (!string.IsNullOrWhiteSpace(filters.SearchText))
                {
                    queryable = queryable.Where(x => x.masjidName.Contains(filters.SearchText)
                                                  || x.masjidAddress.Contains(filters.SearchText)
                                                  || x.masjidPincode.Contains(filters.SearchText)
                                                  || x.masjidMadhab.Contains(filters.SearchText)
                                                  || x.waqthName.Contains(filters.SearchText));
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
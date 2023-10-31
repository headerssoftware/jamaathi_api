using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MWA_API.Data;
using MWA_API.Models;

namespace MWA_API.Controllers
{
    [Route("api/GetMasjidWithUserSubscribeFlag")]
    [ApiController]
    public class SpGetMasjidWithUserSubscribeFlagController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public SpGetMasjidWithUserSubscribeFlagController(ApplicationDbContext context)
        {
            this._context = context;
        }

        [HttpGet("{userId:int}", Name = "getMasjidWithUserSubscribeFlagEmb")]
        public async Task<ActionResult<List<SpGetMasjidWithUserSubscribeFlag>>> GetWaqthEmbedded(int userId)
        {
            try
            {
                var exists = await _context.userMasters.AnyAsync(x => x.userId == userId);
                if (!exists)
                {
                    return BadRequest(new { error = "User Id Not Found" });
                }

                var result = _context.spGetMasjidWithUserSubscribeFlags.FromSql($"EXECUTE dbo.GetMasjidWithUserSubscribeFlag {userId}").ToList();

                foreach (var value in result)
                {
                    if (value.masjidImagePath != null && string.IsNullOrWhiteSpace(value.masjidImagePath) == false)
                    {
                        var fileName = Path.GetFileName(value.masjidImagePath);
                        value.masjidImageURL = $"{Request.Scheme}://{Request.Host.Value}/api/Document/getFile?name={Uri.EscapeDataString(fileName)}";
                    }

                    var viewMasjidWaqths = await _context.viewMasjidWaqths.Where(x => x.masjidId == value.masjidId).ToListAsync();
                    List<ViewMasjidWaqthMaster> waqthdetails = new List<ViewMasjidWaqthMaster>();
                    foreach (var waqthvalue in viewMasjidWaqths)
                    {
                        waqthdetails.Add(new ViewMasjidWaqthMaster
                        {
                            masjidId = waqthvalue.masjidId,
                            masjidWaqthId = waqthvalue.masjidWaqthId,
                            waqthId = waqthvalue.waqthId,
                            waqthName = waqthvalue.waqthName,
                            azanTime = waqthvalue.azanTime,
                            iqaamathTime = waqthvalue.iqaamathTime,
                            startTime = waqthvalue.startTime,
                            endTime = waqthvalue.endTime
                        });

                    }

                    value.waqthDetails= waqthdetails;
                }


 
                return result;
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}

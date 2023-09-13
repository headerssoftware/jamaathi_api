using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MWA_API.Data;
using MWA_API.Models;

namespace MWA_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MasjidMasterController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public IConfiguration _configuration { get; }
        public MasjidMasterController(ApplicationDbContext context, IConfiguration configuration)
        {
            this._context = context;
            this._configuration = configuration;
        }

        [HttpGet]
        public async Task<ActionResult<List<MasjidMaster>>> GetAll()
        {
            try
            {
                var result = await _context.masjidMasters.AsNoTracking().ToListAsync();
                foreach (var value in result)
                {
                    if (value.masjidImagePath != null && string.IsNullOrWhiteSpace(value.masjidImagePath) == false)
                    {
                        
                        var fileName = $"{Path.GetFileName(value.masjidImagePath)}";
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

        [HttpGet("{id:int}", Name = "getMasjidMaster")]
        public async Task<ActionResult<MasjidMaster>> GetById(int id)
        {
            var result = await _context.masjidMasters.FirstOrDefaultAsync(x => x.masjidId == id);
            if (result != null)
            {
                if (result.masjidImagePath != null && string.IsNullOrWhiteSpace(result.masjidImagePath) == false)
                {
                    var fileName = Path.GetFileName(result.masjidImagePath);
                    result.masjidImageURL = $"{Request.Scheme}://{Request.Host.Value}/api/Document/getFile?name={Uri.EscapeDataString(fileName)}";
                }
                return result;
            }

            else
                return NotFound();

        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] MasjidMaster curr)
        {
            try
            {
                _context.Add(curr);
                await _context.SaveChangesAsync();
                return new CreatedAtRouteResult("getMasjidMaster", new { Id = curr.masjidId }, curr);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost]
        [Route("withImage")]
        public async Task<ActionResult> PostWithImage([FromForm] MasjidMaster curr, [FromForm] FilePost? currFile)
        {
            using (var transac = this._context.Database.BeginTransaction())
            {
                try
                {
                    string currDateTimeStr = DateTime.Now.ToString("yyyyMMddHHmmssffff");
                    if (currFile.file != null)
                    {
                        var file = currFile.file;
                        var name = curr.masjidName + "_" + currDateTimeStr + Path.GetExtension(file.FileName);
                        if (string.IsNullOrEmpty(name) == false)
                        {
                            var filename = $"{_configuration.GetValue<string>("fileUploadPath")}\\{name}";
                            using (FileStream fs = System.IO.File.Create(filename))
                            {
                                await file.CopyToAsync(fs);
                                await fs.FlushAsync();
                            }
                        }
                    }



                    var masj = await this._context.masjidMasters.AddAsync(new MasjidMaster()
                    {
                        masjidName = curr.masjidName,
                        masjidLocation = curr.masjidLocation,
                        masjidAddress = curr.masjidAddress,
                        masjidMadhab = curr.masjidMadhab,
                        masjidLastUpdatedTime = curr.masjidLastUpdatedTime,
                        masjidPincode = curr.masjidPincode,
                        masjidImagePath = currFile.file != null ? ($"{_configuration.GetValue<string>("fileUploadPath")}\\" + curr.masjidName + "_" + currDateTimeStr + Path.GetExtension(currFile.file.FileName)) : ""
                        //(currFile != null ? $"{_configuration.GetValue<string>("fileUploadPath")}\\{name}" : "")

                    });

                    await _context.SaveChangesAsync();
                    await transac.CommitAsync();
                    curr.masjidId = masj.Entity.masjidId;

                    if (masj.Entity.masjidImagePath != null && string.IsNullOrWhiteSpace(masj.Entity.masjidImagePath) == false)
                    {
                        var fileName = Path.GetFileName(masj.Entity.masjidImagePath);
                        curr.masjidImageURL = $"{Request.Scheme}://{Request.Host.Value}/api/Document/getFile?name={Uri.EscapeDataString(fileName)}";
                    }
                    return new CreatedAtRouteResult("getMasjidMaster", new { Id = masj.Entity.masjidId }, curr);
                }
                catch (Exception ex)
                {
                    await transac.RollbackAsync();
                    return BadRequest(new { error = ex.Message });
                }
            }

        }
        [Route("{id:int}")]

        [HttpPut]
        public async Task<ActionResult> Put(int id, [FromBody] MasjidMaster curr)
        {
            if (id != curr.masjidId)
            {
                return BadRequest();
            }

            try
            {
                curr.masjidId = id;
                _context.Entry(curr).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut]
        [Route("withImage")]
        public async Task<ActionResult> PutWithImage(int id, [FromForm] MasjidMaster curr, [FromForm] FilePost? currFile)
        {
            if (id != curr.masjidId)
            {
                return BadRequest();
            }
            using (var transac = this._context.Database.BeginTransaction())
            {
                try
                {
                    string currDateTimeStr = DateTime.Now.ToString("yyyyMMddHHmmssffff");
                    if (currFile.file != null)
                    {
                        DeleteMasjidImage(id);
                        var file = currFile.file;
                        var name = curr.masjidName + "_" + currDateTimeStr + Path.GetExtension(file.FileName);
                        if (string.IsNullOrEmpty(name) == false)
                        {
                            var filename = $"{_configuration.GetValue<string>("fileUploadPath")}\\{name}";
                            using (FileStream fs = System.IO.File.Create(filename))
                            {
                                await file.CopyToAsync(fs);
                                await fs.FlushAsync();
                            }
                        }
                    }

                    curr.masjidId = id;
                    curr.masjidImagePath = currFile.file != null ? ($"{_configuration.GetValue<string>("fileUploadPath")}\\" + curr.masjidName + "_" + currDateTimeStr + Path.GetExtension(currFile.file.FileName)) : "";
                    _context.Entry(curr).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    await transac.CommitAsync();
                    return NoContent();

                }
                catch (Exception ex)
                {
                    await transac.RollbackAsync();
                    return BadRequest(new { error = ex.Message });
                }
            }


        }

        [Route("{id:int}")]
        [HttpDelete]
        public async Task<ActionResult> Delete(int id)
        {
            using (var transac = this._context.Database.BeginTransaction())
            {
                try
                {
                    var masjidMasters = await _context.masjidMasters.Where(x => x.masjidId == id).FirstOrDefaultAsync();
                    if (masjidMasters == null)
                    {
                        return NotFound();
                    }

                    var masjidDevice = await _context.masjidDevices.Where(x => x.masjidId == id).ToListAsync();
                    if(masjidDevice!=null)
                    {
                        _context.RemoveRange(masjidDevice);
                    }
                    
                    //Delete the image of the masjid
                    DeleteMasjidImage(id);
                    _context.Remove(masjidMasters);

                    await _context.SaveChangesAsync();
                    await transac.CommitAsync();
                    return NoContent();
                }
                catch (Exception ex)
                {
                    await transac.RollbackAsync();
                    return BadRequest(new { error = ex.Message });
                }
            }
           
        }

        private void DeleteMasjidImage(int masjidId)
        {
            var result = _context.masjidMasters.Where(x => x.masjidId == masjidId).AsNoTracking().ToList();

            foreach (var value in result)
            {
                var path = value.masjidImagePath;
                if (!string.IsNullOrWhiteSpace(path))
                {
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }

            }

        }
    }
}

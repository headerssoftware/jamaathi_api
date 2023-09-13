using System.Net;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MWA_API.Data;
using MWA_API.Enums;
using MWA_API.Models;
using Newtonsoft.Json;
using static System.Net.Mime.MediaTypeNames;

namespace MWA_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MasjidWaqthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public IConfiguration _configuration { get; }
        public MasjidWaqthController(ApplicationDbContext context, IConfiguration configuration)
        {
            this._context = context;
            this._configuration = configuration;
        }

        [HttpGet]
        public async Task<ActionResult<List<MasjidWaqth>>> GetAll()
        {
            return await _context.masjidWaqths.AsNoTracking().ToListAsync();
        }

        [HttpGet("{id:int}", Name = "getMasjidWaqth")]
        public async Task<ActionResult<MasjidWaqth>> GetById(int id)
        {
            var result = await _context.masjidWaqths.FirstOrDefaultAsync(x => x.masjidWaqthId == id);
            if (result != null)
                return result;
            else
                return NotFound();

        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] MasjidWaqth curr)
        {
            try
            {
                _context.Add(curr);
                await _context.SaveChangesAsync();
                return new CreatedAtRouteResult("getMasjidWaqth", new { Id = curr.masjidWaqthId }, curr);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        [Route("{id:int}")]

        [HttpPut]
        public async Task<ActionResult> Put(int id, [FromBody] MasjidWaqth curr)
        {
            if (id != curr.masjidWaqthId)
            {
                return BadRequest();
            }

            using (var transac = this._context.Database.BeginTransaction())
            {
                try
                {
                    var masjid = await _context.masjidMasters.FirstOrDefaultAsync(x => x.masjidId == curr.masjidId);
                    if (masjid == null)
                        return BadRequest("Masjid Not Found!");

                    var oldMasjidWaqth = await _context.masjidWaqths.FirstOrDefaultAsync(x => x.masjidWaqthId == id);
                    if (oldMasjidWaqth == null)
                        return BadRequest("Masjid Waqth Not Found!");

                    var currMasjidWaqth = curr;

                    var notificationMessage = CompareOldNew(oldMasjidWaqth, currMasjidWaqth, masjid.masjidName.ToString());

                    oldMasjidWaqth.masjidId = curr.masjidId;
                    oldMasjidWaqth.waqthId = curr.waqthId;
                    oldMasjidWaqth.azanTime = curr.azanTime;
                    oldMasjidWaqth.iqaamathTime = curr.iqaamathTime;
                    oldMasjidWaqth.startTime = curr.startTime;
                    oldMasjidWaqth.endTime = curr.endTime;

                    masjid.masjidLastUpdatedTime = DateTime.Now;

                   
                   
                    if (!string.IsNullOrWhiteSpace( notificationMessage))
                    {
                        try
                        {
                           await SendMessage(masjid.masjidId.ToString(), notificationMessage);
                        }
                        catch(Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                    }
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
            try
            {
                var exists = await _context.masjidWaqths.AnyAsync(x => x.masjidWaqthId == id);
                if (!exists)
                {
                    return NotFound();
                }
                _context.Remove(new MasjidWaqth() { masjidWaqthId = id });
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        #region helpers
        private class ModifiedWaqth
        {
            public int modifiedMasjidId { get; set; }

            public int modifiedWaqthId { get; set; }
            public List<NewTimings> timings { get; set; }
        }

        private class NewTimings
        {
            public string? timeFor { get; set; }
            public TimeSpan newTime { get; set; }
        }
        private string CompareOldNew(MasjidWaqth dataOld, MasjidWaqth dataNew,string MasjidName)
        {
            var  newtimigs=new List<NewTimings>();
            var strMessage = string.Empty;

            if(!dataOld.azanTime.Equals(dataNew.azanTime) && dataNew.azanTime !=null)
                newtimigs.Add(new NewTimings { timeFor = WaqthSlots.AzanTime.ToString(), newTime = dataNew.azanTime.Value });
            if (!dataOld.iqaamathTime.Equals(dataNew.iqaamathTime) && dataNew.iqaamathTime != null)
                newtimigs.Add(new NewTimings { timeFor = WaqthSlots.IqaamathTime.ToString(), newTime = dataNew.iqaamathTime.Value });
            if (!dataOld.startTime.Equals(dataNew.startTime) && dataNew.startTime != null)
                newtimigs.Add(new NewTimings { timeFor = WaqthSlots.StartTime.ToString(), newTime = dataNew.startTime.Value });
            if (!dataOld.endTime.Equals(dataNew.endTime) && dataNew.endTime != null)
                newtimigs.Add(new NewTimings { timeFor = WaqthSlots.EndTime.ToString(), newTime = dataNew.endTime.Value });

            ModifiedWaqth obj = new ModifiedWaqth();
            if (newtimigs.Count>0)
            {                
                obj.modifiedMasjidId = dataNew.masjidId;
                obj.modifiedWaqthId = dataNew.waqthId;
                obj.timings = newtimigs;

                strMessage = GetMessageString(obj, MasjidName);
            }
            return strMessage;


        }

        private  string GetMessageString (ModifiedWaqth obj, string MasjidName)
        {

            string strMessageTest = "New timings updated for "+ MasjidName + " \n";

            foreach(var value in obj.timings)
            {
                strMessageTest += value.timeFor + " - " + value.newTime.ToString(@"hh\:mm") + "\n";
            }

            return strMessageTest;
            
        }

        #endregion

        #region Notification
       
        private async Task SendMessage(string masjidId,string message)
        {
            try
            {
                List<string> listFcmId = new List<string>();
                var userMasjid=await _context.viewUserMasjids.Where(x => x.masjidId.ToString() == masjidId).AsNoTracking().ToListAsync();
                
                foreach(var item in userMasjid)
                {
                    listFcmId.Add(item.userFcmId);
                }

                string[] registration_ids= listFcmId.ToArray();
                var data = new
                {
                    registration_ids,

                    notification = new
                    {
                        title = "New notification!",
                        body = message
                    }
                };
                SendNotification(data);              
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void SendNotification(object data)
        {
            var json = JsonConvert.SerializeObject(data);
            Byte[] byteArray = Encoding.UTF8.GetBytes(json);
            SendNotification(byteArray);
        }

        private void SendNotification(Byte[] byteArray)
        {
            try
            {

                string serverApiKey = $"{_configuration.GetValue<string>("ServerKey")}";
                string senderId = $"{_configuration.GetValue<string>("SenderId")}";

                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "POST";
                tRequest.ContentType = "application/json";
                tRequest.Headers.Add($"Authorization: Key={serverApiKey}");
                tRequest.Headers.Add($"Sender: id={senderId}");

                tRequest.ContentLength = byteArray.Length;
                Stream dataStream = tRequest.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();

                WebResponse tResponse = tRequest.GetResponse();
                dataStream = tResponse.GetResponseStream();
                StreamReader tReader = new StreamReader(dataStream);

                string sResponseFromServer = tReader.ReadToEnd();

                tReader.Close();
                dataStream.Close();
                tResponse.Close();

            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}

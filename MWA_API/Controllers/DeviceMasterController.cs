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

namespace MWA_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceMasterController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public IConfiguration _configuration { get; }
        public DeviceMasterController(ApplicationDbContext context, IConfiguration configuration)
        {
            this._context = context;
            this._configuration = configuration;
        }

        [HttpGet]
        public async Task<ActionResult<List<DeviceMaster>>> GetAll()
        {
            return await _context.deviceMasters.AsNoTracking().ToListAsync();
        }

        [HttpGet("{id:int}", Name = "getDeviceMaster")]
        public async Task<ActionResult<DeviceMaster>> GetById(int id)
        {
            var result = await _context.deviceMasters.FirstOrDefaultAsync(x => x.deviceId == id);
            if (result != null)
                return result;
            else
                return NotFound();

        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] DeviceMaster curr)
        {
            try
            {
                _context.Add(curr);
                await _context.SaveChangesAsync();
                return new CreatedAtRouteResult("getDeviceMaster", new { Id = curr.deviceId }, curr);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        [Route("{id:int}")]

        [HttpPut]
        public async Task<ActionResult> Put(int id, [FromBody] DeviceMaster curr)
        {
            if (id != curr.deviceId)
            {
                return BadRequest();
            }

            try
            {
                curr.deviceId = id;
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
                var exists = await _context.deviceMasters.AnyAsync(x => x.deviceId == id);
                if (!exists)
                {
                    return NotFound();
                }
                _context.Remove(new DeviceMaster() { deviceId = id });
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        //Create/Update Masjid all Waqths for a Device

        //Action Method
        [HttpPost]
        [Route("withMasjidWaqth")]
        public async Task<ActionResult> PostWithMasjidWaqth([FromBody] MasjidDeviceWaqth curr)
        {
            var notificationMessage = "";
            using (var transac = this._context.Database.BeginTransaction())
            {
                try
                {
                    var deviceMasters = await this._context.deviceMasters.AsNoTracking().FirstOrDefaultAsync(x => x.deviceName == curr.deviceName);

                    if (deviceMasters == null)
                    {
                        return BadRequest("Device name not found!");
                    }

                    var deviceId = deviceMasters.deviceId;

                    var lstWaqthDetails = curr.waqthDetails;

                    if (lstWaqthDetails == null)
                    {
                        return BadRequest("Waqth details not specified!");
                    }

                    if (lstWaqthDetails.Count != 0)
                    {
                        var masjidDevice = await this._context.masjidDevices.AsNoTracking().FirstOrDefaultAsync(x => x.deviceId == deviceId);
                        if (masjidDevice == null)
                        {
                            return BadRequest("Device not mapped to any masjid!");
                        }

                        var masjidId = masjidDevice.masjidId;
                        var masjidMaster = await this._context.masjidMasters.FirstOrDefaultAsync(x => x.masjidId == masjidId);
                        if (masjidMaster == null)
                        {
                            return BadRequest("Masjid not found!");
                        }
                        masjidMaster.masjidLastUpdatedTime = DateTime.Now;

                        notificationMessage = "New timings updated for " + masjidMaster.masjidName + " \n";
                        bool isPuthappened = false;

                        foreach (var waqthdetail in lstWaqthDetails)
                        {
                            var oldMasjidWaqth = await this._context.masjidWaqths.AsNoTracking().FirstOrDefaultAsync(x => x.masjidId == masjidId && x.waqthId == waqthdetail.waqthId);


                            if (oldMasjidWaqth == null)//if waqth not exist add new record else update
                            {
                                var masjidWaqthx = await this._context.masjidWaqths.AddAsync(new MasjidWaqth
                                {
                                    masjidId = masjidId,
                                    waqthId = waqthdetail.waqthId,
                                    azanTime = waqthdetail.azanTime,
                                    iqaamathTime = waqthdetail.iqaamathTime,
                                    startTime = waqthdetail.startTime,
                                    endTime = waqthdetail.endTime
                                });
                            }
                            else
                            {
                                var currMasjidWaqth = await this._context.masjidWaqths.FirstOrDefaultAsync(x => x.masjidId == masjidId && x.waqthId == waqthdetail.waqthId);

                                currMasjidWaqth.azanTime = waqthdetail.azanTime;
                                currMasjidWaqth.iqaamathTime = waqthdetail.iqaamathTime;
                                currMasjidWaqth.startTime = !string.IsNullOrEmpty(waqthdetail.startTime.ToString()) ? waqthdetail.startTime : currMasjidWaqth.startTime;
                                currMasjidWaqth.endTime = !string.IsNullOrEmpty(waqthdetail.endTime.ToString()) ? waqthdetail.endTime : currMasjidWaqth.endTime;


                                var waqthMaster = await this._context.waqthMasters.AsNoTracking().FirstOrDefaultAsync(x => x.waqthId == currMasjidWaqth.waqthId);


                                if (curr.notifyUser == true)
                                {
                                    var newStr = CompareOldNew(oldMasjidWaqth, currMasjidWaqth, waqthMaster.waqthName);
                                    if (newStr.Length > 0)
                                        isPuthappened = true;

                                    notificationMessage += newStr;
                                }


                            }

                        }

                        if (!string.IsNullOrWhiteSpace(notificationMessage) && isPuthappened && curr.notifyUser == true)
                        {
                            try
                            {
                                await SendMessage(masjidId.ToString(), notificationMessage);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.ToString());
                            }
                        }
                        else { notificationMessage = "No notification!"; }

                    }
                    await this._context.SaveChangesAsync();
                    await transac.CommitAsync();

                    return Ok(notificationMessage);
                }
                catch (Exception ex)
                {
                    await transac.RollbackAsync();
                    return BadRequest(new { error = ex.InnerException.Message.ToString() });
                }
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
        private string CompareOldNew(MasjidWaqth dataOld, MasjidWaqth dataNew, string waqthName)
        {
            var newtimigs = new List<NewTimings>();
            var strMessage = string.Empty;

            if (!dataOld.azanTime.Equals(dataNew.azanTime) && dataNew.azanTime != null)
                newtimigs.Add(new NewTimings { timeFor = WaqthSlots.AzanTime.ToString(), newTime = dataNew.azanTime.Value });
            if (!dataOld.iqaamathTime.Equals(dataNew.iqaamathTime) && dataNew.iqaamathTime != null)
                newtimigs.Add(new NewTimings { timeFor = WaqthSlots.IqaamathTime.ToString(), newTime = dataNew.iqaamathTime.Value });
            if (!dataOld.startTime.Equals(dataNew.startTime) && dataNew.startTime != null)
                newtimigs.Add(new NewTimings { timeFor = WaqthSlots.StartTime.ToString(), newTime = dataNew.startTime.Value });
            if (!dataOld.endTime.Equals(dataNew.endTime) && dataNew.endTime != null)
                newtimigs.Add(new NewTimings { timeFor = WaqthSlots.EndTime.ToString(), newTime = dataNew.endTime.Value });

            ModifiedWaqth obj = new ModifiedWaqth();
            if (newtimigs.Count > 0)
            {
                obj.modifiedMasjidId = dataNew.masjidId;
                obj.modifiedWaqthId = dataNew.waqthId;
                obj.timings = newtimigs;

                strMessage = GetMessageString(obj, waqthName);
            }
            return strMessage;


        }

        private string GetMessageString(ModifiedWaqth obj, string waqthName)
        {

            string strMessageTest = waqthName + ":" + "\n";

            foreach (var value in obj.timings)
            {
                strMessageTest += "\t" + value.timeFor + " - " + value.newTime.ToString(@"hh\:mm") + "\n";
            }

            return strMessageTest;

        }

        #endregion

        #region Notification

        private async Task SendMessage(string masjidId, string message)
        {
            try
            {
                List<string> listFcmId = new List<string>();
                var userMasjid = await _context.viewUserMasjids.Where(x => x.masjidId.ToString() == masjidId).AsNoTracking().ToListAsync();

                if (userMasjid.Count > 0)
                {

                    foreach (var item in userMasjid)
                    {
                        listFcmId.Add(item.userFcmId);
                    }

                    string[] registration_ids = listFcmId.ToArray();
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
    #region Create/Update Masjid all Waqths for a Device

    //Models
    public class MasjidDeviceWaqth
    {
        public string deviceName { get; set; }
        public bool notifyUser { get; set; } = true;
        public List<WaqthDetails>? waqthDetails { get; set; }
    }

    public class WaqthDetails
    {
        public int waqthId { get; set; }
        public TimeSpan? azanTime { get; set; }
        public TimeSpan? iqaamathTime { get; set; }
        public TimeSpan? startTime { get; set; }
        public TimeSpan? endTime { get; set; }
    }

    #endregion
}

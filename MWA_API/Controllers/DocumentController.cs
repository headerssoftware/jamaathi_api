using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MWA_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        public DocumentController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        [HttpPost("UploadFile")]
        [RequestSizeLimit(8000000)]
        public async Task<ActionResult> UploadImage([FromForm] IFormFile file, [FromForm] string directory, [FromForm] string name)
        {
            try
            {
                var availableExtension = new string[] { ".jpeg", ".pdf", ".doc", ".jpg", ".png", ".tiff" };
                if (!availableExtension.Contains(Path.GetExtension(file.FileName)))
                {
                    return BadRequest("file extension not matching");
                }


                if (file != null && string.IsNullOrEmpty(name) == false)
                {
                    //  var filename = $"{Configuration.GetValue<string>("fileUploadPath")}\\{name}";
                    var fileDirectory = "";
                    if (!string.IsNullOrEmpty(directory))
                        fileDirectory = Configuration.GetValue<string>("fileUploadPath") + "\\" + directory;
                    else
                        fileDirectory = Configuration.GetValue<string>("fileUploadPath");

                    var filepath = fileDirectory + "\\" + name;


                    if (!Directory.Exists(fileDirectory))
                        Directory.CreateDirectory(fileDirectory);


                    using (FileStream fs = System.IO.File.Create(filepath))
                    {
                        await file.CopyToAsync(fs);
                        await fs.FlushAsync();
                        //Url Friendly Path for the file
                        return Ok(filepath);
                    }
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("getFile")]

        public async Task<ActionResult> getFile(string name)
        {
            try
            {
                var path = $"{Configuration.GetValue<string>("fileUploadPath")}\\{name}";
                if (!System.IO.File.Exists(path))
                {
                    return BadRequest("Document not found");
                }
                FileStream fs = null;
                await Task.Run(() => {
                    fs = System.IO.File.OpenRead(path);
                });

                return File(fs,
                     contentType: path.ToLower().Contains(".jpeg") || path.Contains(".jpg") ? "image/jpeg" :
                      (path.ToLower().Contains(".png") ? "image/png" :
                     (path.ToLower().Contains(".pdf") ? "application/pdf" :
                     (path.ToLower().Contains(".doc") ? "application/msword" :
                     (path.ToLower().Contains(".docx") ? "application/vnd.openxmlformats-officedocument.wordprocessingml.document" :
                     (path.ToLower().Contains(".tiff") ? "image/tiff" : "octet-stream"
                    ))))));

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest("Sorry unable to read file!!");
            }
        }

        [HttpDelete]
        public async Task<ActionResult> deleteFile(string fileName)
        {
            try
            {
                var path = $"{Configuration.GetValue<string>("fileUploadPath")}\\{fileName}";
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
               
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
            return NoContent();
        }
    }
}

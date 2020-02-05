using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using web2.Data;

namespace web2.Controllers
{
    [Route("api/[controller]")]
    public class DibUploadController : Controller
    {
        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> Post(List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);

            foreach (IFormFile formFile in files)
                await handleSingleFileUpload(formFile);

            return Ok(new { count = files.Count, size });
        }

        async Task handleSingleFileUpload(IFormFile formFile)
        {
            Uploader uploader = new Uploader();
            uploader.Upload(formFile);
        }
    }
}

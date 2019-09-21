using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
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
        public async Task<IActionResult> Post(List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);

            foreach (IFormFile formFile in files)
                handleSingleFileUpload(formFile);

            return Ok(new { count = files.Count, size });
        }

        void handleSingleFileUpload(IFormFile formFile)
        {
            Uploader uploader = new Uploader();
            uploader.Upload(formFile);
        }
    }
}

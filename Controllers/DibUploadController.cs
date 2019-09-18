using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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
            if (formFile.Length == 0)
                return;
            
            string filePath = Path.GetFileNameWithoutExtension(formFile.FileName) + "/" + formFile.FileName;

            System.Console.WriteLine(filePath);
            using (FileStream stream = new FileStream(filePath, FileMode.Create))
                formFile.CopyTo(stream);
        }

        void createDirectoryIfDoesntExist(string filePath)
        {
            string directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
        }
    }
}

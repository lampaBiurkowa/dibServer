using Microsoft.AspNetCore.Mvc;
using web2.Data;

namespace web2.Controllers
{
    [Route("api/[controller]")]
    public class DibInfoController : Controller
    {
        [HttpGet("getVersionInfo/{appName}")]
        public int GetVersionInfo(string appName)
        {
            return AppVersionHandler.GetVersion(appName);
        }
    }
}
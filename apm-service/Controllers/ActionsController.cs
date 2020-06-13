using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using apm_get;
using apm_service.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace apm_service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActionsController : ControllerBase
    {
        [HttpGet]
        public List<PackageManifest> Get()
        {
            return PackageList.Packages;
        }

        [HttpGet("{app}")]
        public PackageManifest Get(string app)
        {
            if (PackageList.PackagesDic.ContainsKey(app))
            return  PackageList.PackagesDic[app];
            return new PackageManifest() { AppName = "" };
        }

        [HttpGet("{app}/{fn}")]
        [Route("Stream")]
        public IActionResult Get(string app, int fn)
        {
            var a = PackageList.PackagesDic[app];
            string fl = Path.Combine(a.Directory, a.Actions[fn].Content);
            Stream stream = System.IO.File.OpenRead(fl);
            string mimeType = "application/bin";
            return new FileStreamResult(stream, mimeType)
            {
                FileDownloadName = Path.GetFileName(fl)
            };

        }
    }
}

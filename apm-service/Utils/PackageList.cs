using apm_get;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System;

namespace apm_service.Utils
{
    public static class PackageList
    {
        public static readonly List<PackageManifest> Packages = new List<PackageManifest>();
        public static readonly Dictionary<string,PackageManifest> PackagesDic = new Dictionary<string,PackageManifest>(StringComparer.OrdinalIgnoreCase);

        static PackageList()
        {
            DirectoryInfo di = new DirectoryInfo("ApmPackages");
            var dd = di.GetDirectories();
            foreach (var d in dd)
            {
                var fn = Path.Combine(d.FullName, "Manifest.mapm");                
                if (!File.Exists(fn)) continue;
                var fs = File.ReadAllText(fn);
                var p = JsonSerializer.Deserialize<PackageManifest>(fs);
                p.Directory = d.FullName;
                Packages.Add(p);
                PackagesDic.Add(p.AppName, p);

            }
        }
    }
}

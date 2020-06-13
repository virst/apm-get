using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace apm_get
{
    class Program
    {
        static Config c;
        const string fn = "ApmConfig.json";
        static readonly Dictionary<string, MethodInfo> methods = new Dictionary<string, MethodInfo>(StringComparer.OrdinalIgnoreCase);
        static WebClient webClient = new WebClient();

        static Program()
        {
            var s = File.ReadAllText(fn);
            c = JsonSerializer.Deserialize<Config>(s);
            var t = typeof(Program);
            var mm = t.GetMethods();
            foreach (var m in mm)
                if (m.IsPublic && m.IsStatic)
                    methods[m.Name] = m;
        }

        static void Main(string[] args)
        {
            if (args.Length == 0) return;
            if (!methods.ContainsKey(args[0]))
            {
                Console.WriteLine("Не известная команда !");
                return;
            }
            methods[args[0]].Invoke(null, args[1..]);
        }

        public static void List()
        {
            var s = webClient.DownloadString(c.ApiUrl + @"/Actions");
            s = s.Replace("appName", "AppName");
            s = s.Replace("dependencies", "Dependencies");
            s = s.Replace("actions", "Actions");
            List<PackageManifest> ps = JsonSerializer.Deserialize<List<PackageManifest>>(s);
            foreach (var p in ps)
                Console.WriteLine(p.AppName);
        }

        public static void Install(string app)
        {
            var s = webClient.DownloadString(c.ApiUrl + @$"/Actions/{app}");
            s = s.Replace("appName", "AppName");
            s = s.Replace("dependencies", "Dependencies");
            s = s.Replace("actions", "Actions");
            s = s.Replace("type", "Type");
            s = s.Replace("content", "Content");
            PackageManifest p = JsonSerializer.Deserialize<PackageManifest>(s);
            Console.WriteLine("Installing = {0}", p.AppName);
            var pth = Path.Combine(c.AppFolder, p.AppName);
            Directory.CreateDirectory(pth);
            for (int i = 0; i < p.Actions.Count;)
            {
                var a = p.Actions[i++];
                Console.WriteLine("{0}/{1}", i, p.Actions.Count);
                switch (a.Type)
                {
                    case PackageManifest.ActionType.Copy:
                        webClient.DownloadFile(c.ApiUrl + @$"/Actions/{app}/{i - 1}", Path.Combine(pth, a.Content));
                        break;
                    case PackageManifest.ActionType.Delete:
                        break;
                    case PackageManifest.ActionType.Run:
                        Process.Start(Path.Combine(pth, a.Content));
                        break;
                }
            }
        }
    }
}

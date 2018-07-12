using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Seppuku.Module.Config;
using System.Reflection;
using Newtonsoft.Json.Linq;
using Seppuku.Module;
using Seppuku.Module.Standard;

namespace Seppuku.Client.Linux.RunOnce
{
    internal class Program
    {
        public static int Main(string[] args)
        {
            // quick and dirty runonce app to be utilized with cron
            TypeConf Config = new TypeConf(Path.Combine(Directory.GetParent(Assembly.GetEntryAssembly().Location).FullName, "Seppuku.Client.Linux.RunOnce.json"));
            Config.Initialize(new Dictionary<string, object>
            {
                ["Secret"] = "",
                ["RemoteAddress"] = "http://localhost",
                ["Port"] = 19007L,
            });
            string token = SeppukuModule.GetCurrentToken(Config.Get("Secret", ""));
            Console.WriteLine($"{token}");
            string addr = $"{Config.Get("RemoteAddress", "")}:" +
                          $"{Config.Get("Port", 0L)}/reset/{token}";
            string data = new WebClient().DownloadString(addr);

            int ret = (int)JObject.Parse(data)["status"];

            if (ret != (int) StatusCode.Success)
                return 1;
            return 0;
        }
    }
}
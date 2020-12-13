using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ElectronNET.API;

namespace emojipad
{
    public class Program
    {
        public static IHost EHost;
        public static bool Running = false;
        public static void Main(string[] args)
        {
            EHost = CreateHostBuilder(args).Build();
            Running = true;
            EHost.Run();
            Running = false;
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(l =>
                {
                    l.ClearProviders();
                    l.AddFile("logs/emojipad.log");
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseElectron(args);
                    webBuilder.UseStartup<Startup>();
                });
    }
}

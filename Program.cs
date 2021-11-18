using EmojiPad.Data;
using EmojiPad.Services;
using EmojiPad.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace EmojiPad
{
    internal class Program
    {
        public static IServiceProvider Services;
        [STAThread]
        static void Main(string[] args)
        {
            var mtx = new Mutex(true, "Encodeous-EmojiPad", out var isNew);
            if (!isNew)
            {
                return;
            }
            var host = CreateHostBuilder(args).Build();
            Services = host.Services;
            host.Start();
            var app = new App();
            app.InitializeComponent();
            app.Run();
            ServiceHost.Instance.Stop();
        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                var def = EmojiPadConfiguration.CreateDefault();
                var cfg = EmojiPadConfiguration.Load();

                if (!Directory.Exists(cfg.EmojiFolderPath))
                {
                    Directory.CreateDirectory(cfg.EmojiFolderPath);
                }
                if (!Directory.Exists(cfg.EmojiDatabasePath))
                {
                    Directory.CreateDirectory(cfg.EmojiDatabasePath);
                }

                if (cfg.FrequentEmojiCount > 1000)
                {
                    cfg.FrequentEmojiCount = 1000;
                }

                if (cfg.EmojiPasteSize > 10000)
                {
                    cfg.EmojiPasteSize = 10000;
                }
                cfg.Save();
                services.AddSingleton(cfg);
                var db = new SQLiteConnection(Path.Join(cfg.EmojiDatabasePath, "emojis.db"));
                db.CreateTable<Emoji>();
                services.AddSingleton(db);
                services.AddSingleton<SearchService>();
                services.AddSingleton<EmojiPadModel>();
                services.AddSingleton<FileService>();
                services.AddSingleton<StatusService>();
                services.AddSingleton<WindowService>();
                services.AddSingleton<EventService>();
                services.AddSingleton<KeyboardHook>();
                services.AddHostedService<ServiceHost>();
            });
    }
}

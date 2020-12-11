using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ElectronNET.API;
using ElectronNET.API.Entities;
using emojipad.Services;
using emojipad.Shared;
using MatBlazor;
using Material.Blazor;
using Microsoft.JSInterop;

namespace emojipad
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static IConfiguration Configuration { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddScoped<ClipboardService>();
            services.AddSingleton<WindowService>();
            services.AddSingleton<SearchService>();
            services.AddSingleton<FileService>();
            services.AddSingleton<EmojiContext>();
            services.AddSingleton<StatusService>();
            services.AddSingleton<EventService>();
            services.AddMBServices(
                toastServiceConfiguration: new MBToastServiceConfiguration()
                {
                    InfoDefaultHeading = "Info",
                    SuccessDefaultHeading = "Success",
                    WarningDefaultHeading = "Warning",
                    ErrorDefaultHeading = "Error",
                    Timeout = 5000,
                    MaxToastsShowing = 5
                },

                animatedNavigationManagerServiceConfiguration: new MBAnimatedNavigationManagerServiceConfiguration()
                {
                    ApplyAnimation = true,
                    AnimationTime = 300
                }
            );
            services.AddMatBlazor();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // "Warm Up" Services
            app.ApplicationServices.GetService<SearchService>();
            app.ApplicationServices.GetService<FileService>();
            var wndsvc = app.ApplicationServices.GetService<WindowService>();
            // Setup aspnet stuff
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }
            app.UseStaticFiles();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(name: "default", pattern: "{controller}/{action}");
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
            
            
            // Electron
            Electron.App.Ready += wndsvc.OnLoad;
            // Electron.App.SetLoginItemSettings(new LoginSettings()
            // {
            //     OpenAtLogin = Convert.ToBoolean(Configuration["ems:run-on-start"]),
            //     Path = Utilities.GetExecutingFile()
            // });
        }
    }
}

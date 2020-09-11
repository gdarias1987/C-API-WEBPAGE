using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HealthChecks.System;
using HealthChecks.UI.Client;
using MELI.Helpers;
using MELI.Services;
using MELI.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MELI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
                .AddCookie(options =>
            {
                options.LoginPath = "/Login";
                options.LogoutPath = "/";
                options.AccessDeniedPath = "/Error";
            });

            services.AddCors(); // DEV JWT

            services.AddHttpContextAccessor();

            services.AddRazorPages().AddRazorRuntimeCompilation(); //  ADDED

            services.AddRazorPages()
                .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null); // ADDED

            services.AddControllers()
                .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null); // ADDED

            services.AddHealthChecks()
                .AddMySql(Configuration["ConnectionString:MySQL"])
                .AddDiskStorageHealthCheck(delegate(DiskStorageOptions diskStorageOptions){
                    diskStorageOptions.AddDrive(@"C:\", 1000);
                },"SSD_Drive",HealthStatus.Degraded);

            string serviceType = Configuration["DataService"];

            //ADDES
            switch (serviceType)
            {
                case "MySQL":
                    services.AddSingleton<IDataService, DataServiceMySQL>();
                    break;
                default:
                    services.AddSingleton<IDataService, DataServiceDynamic>();
                    break;
            }

            services.AddSingleton<ILoggerService, LoggerService>();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                    app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseMiddleware<JwtMiddleware>(); // ADDED

            app.UseAuthentication(); // ADDED
            app.UseAuthorization();

            app.UseHealthChecks("/hc", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter =    UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
            });
        }
    }
}

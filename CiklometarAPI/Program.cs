using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CiklometarBLL;
using CiklometarBLL.Services;
using CiklometarDAL;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CiklometarAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope())
            {
                //var services = scope.ServiceProvider;
                var context = scope.ServiceProvider.GetService<CiklometarContext>();
                SeedData.SeedSuperAdmin(context);
            }
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}

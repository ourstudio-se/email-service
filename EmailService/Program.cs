﻿using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace EmailService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }                                                                               

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseKestrel()
                .UseStartup<Startup>()
                .UseUrls("http://localhost:51101");
        }
    }
}

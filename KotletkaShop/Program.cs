using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using KotletkaShop.Data;
using System.Globalization;

namespace KotletkaShop
{
	public class Program
	{
		public static void Main(string[] args)
		{
			IWebHost host = CreateWebHostBuilder(args).Build();

			// Инициализация БД
			using (IServiceScope scope = host.Services.CreateScope())
			{
				IServiceProvider services = scope.ServiceProvider;
				try
				{
					StoreContext context = services.GetRequiredService<StoreContext>();
					DbInitializer.Initialize(context);
				}
				catch (Exception ex)
				{
					var logger = services.GetRequiredService<ILogger<Program>>();
					logger.LogError(ex, "An error occurred while seeding the database.");
				}
			}

			CultureInfo.CurrentCulture = new CultureInfo("ru-RU", false);

			host.Run();
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.UseStartup<Startup>();
	}
}

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using SixLabors.ImageSharp.Web.Caching;
using SixLabors.ImageSharp.Web.Commands;
using SixLabors.ImageSharp.Web.DependencyInjection;
using SixLabors.ImageSharp.Web.Middleware;
using SixLabors.ImageSharp.Web.Processors;
using SixLabors.ImageSharp.Web.Providers;
using SixLabors.Memory;

namespace ImageServies
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.Configure<CookiePolicyOptions>(options =>
			{
				// This lambda determines whether user consent for non-essential cookies is needed for a given request.
				options.CheckConsentNeeded = context => true;
				options.MinimumSameSitePolicy = SameSiteMode.None;
			});


			//services.AddImageSharpCore()
			//	.SetRequestParser<QueryCollectionRequestParser>()
			//	.SetMemoryAllocatorFromMiddlewareOptions()
			//	.SetCache(provider => new PhysicalFileSystemCache(
			//		provider.GetRequiredService<Microsoft.AspNetCore.Hosting.IHostingEnvironment>(),
			//		provider.GetRequiredService<MemoryAllocator>(),
			//		provider.GetRequiredService<IOptions<ImageSharpMiddlewareOptions>>())
			//	{
			//		Settings = { [PhysicalFileSystemCache.Folder] = PhysicalFileSystemCache.DefaultCacheFolder }
			//	})
			//	.SetCacheHash<CacheHash>()
			//	.AddProvider<PhysicalFileSystemProvider>()
			//	.AddProcessor<ResizeWebProcessor>()
			//	.AddProcessor<FormatWebProcessor>()
			//	.AddProcessor<BackgroundColorWebProcessor>();

			// Add the default service and options.
			//
			services.AddImageSharp();

			// Or add the default service and custom options.
			//
			// this.ConfigureDefaultServicesAndCustomOptions(services);

			// Or we can fine-grain control adding the default options and configure all other services.
			//
			// this.ConfigureCustomServicesAndDefaultOptions(services);

			// Or we can fine-grain control adding custom options and configure all other services
			// There are also factory methods for each builder that will allow building from configuration files.
			//
			// this.ConfigureCustomServicesAndCustomOptions(services);


			services.AddMvc()
				.AddNewtonsoftJson();
		}


		private void ConfigureDefaultServicesAndCustomOptions(IServiceCollection services)
		{
			services.AddImageSharp(
				options =>
				{
					options.Configuration = SixLabors.ImageSharp.Configuration.Default;
					options.MaxBrowserCacheDays = 7;
					options.MaxCacheDays = 365;
					options.CachedNameLength = 8;
					options.OnParseCommands = _ => { };
					options.OnBeforeSave = _ => { };
					options.OnProcessed = _ => { };
					options.OnPrepareResponse = _ => { };
				});
		}

		private void ConfigureCustomServicesAndDefaultOptions(IServiceCollection services)
		{
			services.AddImageSharpCore()
					.SetRequestParser<QueryCollectionRequestParser>()
					.SetMemoryAllocator<ArrayPoolMemoryAllocator>()
					.SetCache<PhysicalFileSystemCache>()
					.SetCacheHash<CacheHash>()
					.AddProvider<PhysicalFileSystemProvider>()
					.AddProcessor<ResizeWebProcessor>()
					.AddProcessor<FormatWebProcessor>()
					.AddProcessor<BackgroundColorWebProcessor>();
		}

		private void ConfigureCustomServicesAndCustomOptions(IServiceCollection services)
		{
			services.AddImageSharpCore(
				options =>
				{
					options.Configuration = SixLabors.ImageSharp.Configuration.Default;
					options.MaxBrowserCacheDays = 7;
					options.MaxCacheDays = 365;
					options.CachedNameLength = 8;
					options.OnParseCommands = _ => { };
					options.OnBeforeSave = _ => { };
					options.OnProcessed = _ => { };
					options.OnPrepareResponse = _ => { };
				})
				.SetRequestParser<QueryCollectionRequestParser>()
				.SetMemoryAllocator(provider => ArrayPoolMemoryAllocator.CreateWithMinimalPooling())
				.SetCache(provider =>
				{
					var p = new PhysicalFileSystemCache(
						provider.GetRequiredService<Microsoft.AspNetCore.Hosting.IHostingEnvironment>(),
						provider.GetRequiredService<MemoryAllocator>(),
						provider.GetRequiredService<IOptions<ImageSharpMiddlewareOptions>>());

					p.Settings[PhysicalFileSystemCache.Folder] = PhysicalFileSystemCache.DefaultCacheFolder;

					return p;
				})
				.SetCacheHash<CacheHash>()
				.AddProvider<PhysicalFileSystemProvider>()
				.AddProcessor<ResizeWebProcessor>()
				.AddProcessor<FormatWebProcessor>()
				.AddProcessor<BackgroundColorWebProcessor>();
		}


		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();

			//app.UseDefaultFiles();
			app.UseImageSharp();
			app.UseStaticFiles();

			app.UseRouting(routes =>
			{
				routes.MapControllerRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");

				routes.MapRazorPages();
			});

			app.UseCookiePolicy();

			app.UseAuthorization();
		}
	}
}

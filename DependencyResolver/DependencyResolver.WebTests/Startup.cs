using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DependencyResolver.Web.Extensions;
using DependencyResolver.Web.Provider;
using DependencyResolver.WebTests.Subjects;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

namespace DependencyResolver.WebTests
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .AddNewtonsoftJson();

            services.AddScoped<SubjectTest>();

            var provider = services
               //.BuildServiceProvider();
               .BuildDependencyResolver()
               .SetResolver(new Resolvers.NameResolver());


            //var a0 = provider.GetService(typeof(IEnumerable<IConfigureOptions<ConsoleLoggerOptions>>));
            //var a1 = provider.GetService(typeof(IEnumerable<IPostConfigureOptions<ConsoleLoggerOptions>>));
            //var a2 = provider.GetService(typeof(IEnumerable<IValidateOptions<ConsoleLoggerOptions>>));

            //TestType(services, provider, typeof(Microsoft.Extensions.Hosting.IHostApplicationLifetime));
            //TestType(services, provider, typeof(Microsoft.AspNetCore.Authentication.IAuthenticationSchemeProvider));
            //TestType(services, provider, typeof(IOptions<>));
            //TestType(services, provider, typeof(Microsoft.Extensions.Options.IOptions<Microsoft.AspNetCore.Builder.IISServerOptions>));

            //TestType(services, provider, typeof(Microsoft.AspNetCore.Http.IHttpContextAccessor));

            //TestType(services, provider, typeof(INameSubject));

            return provider;
        }

        private void TestType(IServiceCollection services, IServiceProvider provider, Type type)
        {
            var hasDescription = services
                .FirstOrDefault(s => s.ServiceType == type);

            var resolve = provider.GetService(type);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting(routes =>
            {
                routes.MapControllers();
            });

            app.UseAuthorization();
        }
    }
}

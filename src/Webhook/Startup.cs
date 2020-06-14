using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Reflection;
using System.Text.Json.Serialization;
using Webhook.Configuration;
using Webhook.Infrastructure;

namespace Webhook
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AppSettings>(Configuration.GetSection(nameof(AppSettings)));

            AppSettings appSettings = new AppSettings();
            Configuration.GetSection(nameof(AppSettings)).Bind(appSettings);

            services.AddCors();
            services.AddControllers(options =>
            {
                options.Filters.Add(typeof(HttpGlobalExceptionFilter));
            })
                .AddJsonOptions(options =>
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

            services.AddAutoMapper(Assembly.Load(typeof(Startup).Assembly.GetName().Name!));
            services.AddSwagger("1.0");

            services.AddHealthChecks();
            services.AddDependencies(appSettings);
            services.AddHttpContextAccessor();

            // Autorise l'accès à des ressources gRPC en HTTP.
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
        }

        public void Configure(IApplicationBuilder app, IOptions<AppSettings> appSettings, IMapper mapper)
        {
            mapper.ConfigurationProvider.AssertConfigurationIsValid();

          
            app.UseDeveloperExceptionPage();
            app.UseSwaggerConfig("1.0", "", "http");
            

            app.UseRouting();
            app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/hc");
                endpoints.MapHealthChecks("/liveness");
            });
        }
    }
}

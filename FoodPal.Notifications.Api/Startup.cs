using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MassTransit;
using FoodPal.Notifications.Common.Settings;
using MassTransit.Azure.ServiceBus.Core.Configurators;
using System;
using Microsoft.Azure.ServiceBus.Primitives;

namespace FoodPal.Notifications.Api
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
            var messageBrokerSettings = Configuration.GetSection("MessageBroker").Get<MessageBrokerSettings>(); 

            services.AddMassTransit(configure => 
            {
                configure.SetKebabCaseEndpointNameFormatter();
                configure.UsingAzureServiceBus((context, config) =>
                {
                    config.Host(messageBrokerSettings.ServiceBusHost);
                    config.ConfigureEndpoints(context);
                });
            });

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "FoodPal.Notifications.Api", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FoodPal.Notifications.Api v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

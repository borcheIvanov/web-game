using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSwag;
using NSwag.AspNetCore;
using NSwag.Generation.Processors.Security;
using web_game.Models;
using web_game.Repositories;
using web_game.Services;

namespace web_game
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
            services.AddControllers();

            var authConfig = new AuthConfig();
            Configuration.Bind("AzureAd", authConfig);

            services.AddAuthentication(options => options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftAccount(microsoftOptions =>
                {
                    microsoftOptions.ClientId = authConfig.ClientId;
                    microsoftOptions.ClientSecret = authConfig.ClientSecret;
                })
                .AddJwtBearer(opt =>
                {
                    opt.Audience = authConfig.ClientId;
                    opt.Authority = $"{authConfig.Instance}{authConfig.TenantId}";
                });


            services.AddScoped<IRepository, Repository>();
            services.AddTransient<IMatchesService, MatchesService>();

            services.AddOpenApiDocument((options, s) =>
            {
                options.Title = "Web Game";
                options.Version = "v1";

                options.AddSecurity("oauth2", Enumerable.Empty<string>(), new OpenApiSecurityScheme
                {
                    AuthorizationUrl = $"{authConfig.Authority}/oauth2/v2.0/authorize",
                    Flow = OpenApiOAuth2Flow.Implicit,
                    TokenUrl = $"{authConfig.Authority}/oauth2/v2.0/token",
                    // "https://login.microsoftonline.com/eb185d82-855f-4910-91b4-36603df8fce9/oauth2/v2.0/token",
                    Type = OpenApiSecuritySchemeType.OAuth2,
                    Scopes = new Dictionary<string, string>
                    {
                        {
                            "openid", "open Id"
                        }
                    },
                    Scheme = JwtBearerDefaults.AuthenticationScheme
                });
            
                options.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("oauth2"));
            });

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios,
                // see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            var authConfig = new AuthConfig();
            Configuration.Bind("AzureAd", authConfig);

            app.UseOpenApi();
            
            app.UseSwaggerUi3(options =>
            {
                options.OAuth2Client = new OAuth2ClientSettings
                {
                    ClientId = authConfig.ClientId,
                    AppName = "Web Game",
                    ClientSecret = authConfig.ClientSecret
                };
            });
      
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
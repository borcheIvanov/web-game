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
using web_game.Repositories;
using web_game.Services;

namespace web_game {
    public class Startup {
        public Startup (IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices (IServiceCollection services) {
            services.AddControllers ();

            var authConfig = new AuthConfig ();
            Configuration.Bind ("IdentityServerAuth", authConfig);

            services.AddAuthentication ("Bearer")
                .AddJwtBearer ("Bearer", options => {
                    options.Authority = authConfig.Authority;
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.Audience = authConfig.Audience;
                });

            services.AddScoped<IRepository, Repository> ();
            services.AddScoped<IService, Service> ();

            services.AddCors (options => options.AddPolicy ("CorsPolicy", builder => builder.AllowAnyOrigin ()
                .AllowAnyMethod ().AllowAnyHeader ()));

            services.AddOpenApiDocument ((options, s) => {
                options.Title = "Web Game";
                options.Version = "v1";

                options.AddSecurity ("oauth2", Enumerable.Empty<string> (), new OpenApiSecurityScheme {
                    AuthorizationUrl = $"{authConfig.Authority}/connect/authorize",
                        Flow = OpenApiOAuth2Flow.Implicit,
                        TokenUrl = $"{authConfig.Authority}/connect/token",
                        Type = OpenApiSecuritySchemeType.OAuth2,
                        Scopes = new Dictionary<string, string> {
                            {
                                "game-rw",
                                "Game-Read/Write"
                            }
                        },
                        Scheme = JwtBearerDefaults.AuthenticationScheme
                });

                options.OperationProcessors.Add (new AspNetCoreOperationSecurityScopeProcessor ("oauth2"));
            });

            services.AddControllers ();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure (IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment ()) {
                app.UseDeveloperExceptionPage ();
            } else {
                app.UseExceptionHandler ("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios,
                // see https://aka.ms/aspnetcore-hsts.
                app.UseHsts ();
            }

            // app.UseHttpsRedirection();
            app.UseCors ("CorsPolicy");

            app.UseRouting ();

            app.UseAuthentication ();
            app.UseAuthorization ();

            var authConfig = new AuthConfig ();
            Configuration.Bind ("IdentityServerAuth", authConfig);

            app.UseOpenApi ();
            app.UseSwaggerUi3 (options => {
                options.OAuth2Client = new OAuth2ClientSettings {
                    ClientId = authConfig.ClientId,
                    ClientSecret = authConfig.ClientSecret
                };
            });

            app.UseEndpoints (endpoints => { endpoints.MapControllers (); });
        }
    }
}
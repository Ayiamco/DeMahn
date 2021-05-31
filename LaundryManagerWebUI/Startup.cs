using LaundryManagerAPIDomain.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LaundryManagerAPIDomain.Services;
using Microsoft.OpenApi.Models;
using LaundryManagerAPIDomain.Contracts;
using LaundryManagerAPIDomain.Queries;
using LaundryManagerWebUI.Interfaces;
using LaundryManagerWebUI.Infrastructure;
using LaundryManagerWebUI.Services;
using LaundryManagerAPIDomain.Services.EmailService;

namespace LaundryManagerWebUI
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
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "LaundryManagerApi", Version = "v1" });
            });

            
            services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                        b => b.MigrationsAssembly("LaundryManagerWebUI")));
           
            services.AddIdentity<ApplicationUser, IdentityRole>(opt=>
            {
                opt.User.RequireUniqueEmail = true;
                opt.SignIn.RequireConfirmedEmail = true;
            }).AddEntityFrameworkStores<ApplicationDbContext>();

            var emailConfig = Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
            services.AddSingleton(emailConfig);
            services.AddSingleton<IJWTManager,JWTAuthManager>();

            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<ISaveChanges, UnitOfWork>();
            services.AddScoped<IJWTManager, JWTAuthManager>();
            services.AddScoped<IIdentityQuery, IdentityQuery>();
            services.AddScoped<IAuthService, AuthService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "LaundryManagerApi v1"));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            SeedAdmin.EnsurePopulated(app, Configuration);
        }
    }
}

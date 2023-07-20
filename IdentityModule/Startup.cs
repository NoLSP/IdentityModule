using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityModule.Database;
using IdentityModule.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IdentityModule
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
            services.AddDbContext<IdentityDataContext>(options => options.UseNpgsql(Configuration.GetConnectionString("DatabaseConnection")));

            services.AddScoped<UserStore<User, Role, IdentityDataContext, long>, MyUserStore> ();
            services.AddScoped<UserManager<User>, MyUserManager>();
            services.AddScoped<RoleManager<Role>, MyRoleManager>();
            services.AddScoped<SignInManager<User>, MySignInManager>();
            services.AddScoped<RoleStore<Role, IdentityDataContext, long>, MyRoleStore>();

            services.AddIdentity<User, Role>()
                .AddUserStore<MyUserStore>()
                .AddUserManager<MyUserManager>()
                .AddRoleStore<MyRoleStore>()
                .AddRoleManager<MyRoleManager>()
                .AddSignInManager<MySignInManager>();

            services.AddControllersWithViews();
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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}

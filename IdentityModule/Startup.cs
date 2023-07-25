using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityModule.Authorize;
using IdentityModule.Database;
using IdentityModule.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
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

            services.AddIdentity<User, Role>( options => {
                    options.User.RequireUniqueEmail = true;
                }).AddEntityFrameworkStores<IdentityDataContext>()
                .AddDefaultTokenProviders()
                .AddDefaultUI();
            
            services.Configure<IdentityOptions>(opt =>
            {
                opt.Password.RequiredLength = 6;
                opt.Password.RequireLowercase = true;
                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromSeconds(30);
                opt.Lockout.MaxFailedAccessAttempts = 5;
            });

            //services.ConfigureApplicationCookie(opt =>
            //{
            //    opt.AccessDeniedPath = new Microsoft.AspNetCore.Http.PathString("/Home/AccessDenied");
            //});

            //services.AddAuthentication().AddGoogle(options =>
            //{
            //    options.ClientId = "";
            //    options.ClientSecret = "";
            //});

            services.AddAuthorization(options =>
            {
                options.AddPolicy(PolicyNames.Administrator, policy => policy.Requirements.Add(PolicyRequirements.Administartor));
                options.AddPolicy(PolicyNames.User, policy => policy.Requirements.Add(PolicyRequirements.User));
                options.AddPolicy(PolicyNames.Developer, policy => policy.Requirements.Add(PolicyRequirements.Developer));

                options.AddPolicy(PolicyNames.Admin_CreateAccess, policy => policy.RequireRole(RoleNames.Administrator)
                    .RequireClaim(ClaimNames.Create, true.ToString()));

                options.AddPolicy(PolicyNames.Admin_Create_Edit_DeleteAccess, policy => policy.RequireRole(RoleNames.Administrator)
                    .RequireClaim(ClaimNames.Create, true.ToString())
                    .RequireClaim(ClaimNames.Edit, true.ToString())
                    .RequireClaim(ClaimNames.Delete, true.ToString()));

                options.AddPolicy(PolicyNames.Admin_Create_Edit_DeleteAccess_OR_Developer, policy => policy.RequireAssertion(context =>
                    AuthorizeAdminWithClaimsOrSuperAdmin(context)));

                options.AddPolicy(PolicyNames.AdminWithMoreThan1000Days, policy => policy.Requirements.Add(new AdminWithMoreThan1000DaysRequirement(1000)));

                options.AddPolicy(PolicyNames.FirstNameAuth, policy => policy.Requirements.Add(new FirstNameAuthRequirement("billy")));
            });

            services.AddScoped<IAuthorizationHandler, AdminWithOver1000DaysHandler>();
            services.AddScoped<IAuthorizationHandler, FirstNameAuthHandler>();
            services.AddScoped<INumberOfDaysForAccount, NumberOfDaysForAccount>();

            services.AddControllersWithViews();
            services.AddRazorPages();
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

                endpoints.MapRazorPages();
            });
        }

        private bool AuthorizeAdminWithClaimsOrSuperAdmin(AuthorizationHandlerContext context)
        {
            return (
                    context.User.IsInRole(RoleNames.Administrator) && 
                    context.User.HasClaim(c => c.Type == ClaimNames.Create && c.Value == true.ToString()) && 
                    context.User.HasClaim(c => c.Type == ClaimNames.Edit && c.Value == true.ToString()) && 
                    context.User.HasClaim(c => c.Type == ClaimNames.Delete && c.Value == true.ToString())
                ) || context.User.IsInRole(RoleNames.Developer);
        }
    }
}

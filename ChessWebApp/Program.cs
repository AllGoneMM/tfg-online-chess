using System.Globalization;
using ChessLibrary;
using ChessWebApp.Hubs;
using ChessWebApp.Identity;
using ChessWebApp.Models.ViewModels;
using ChessWebApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ChessWebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("ChessIdentity") ?? throw new InvalidOperationException("Connection string 'ChessIdentity' not found.");
            builder.Services.AddControllersWithViews()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization();
                
            builder.Services.AddLocalization(options => options.ResourcesPath = "Data/Resources");
            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new List<CultureInfo>
                {
                    new CultureInfo("en-US"),
                    new CultureInfo("es-ES")
                };
                options.DefaultRequestCulture = new RequestCulture("en-US");
                options.SupportedCultures = supportedCultures;
            });
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddDbContext<ChessIdentityDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });
            builder.Services
                .AddIdentity<ChessUser, ChessRole>(
                    options =>
                    {
                        options.SignIn.RequireConfirmedEmail = false;
                        options.SignIn.RequireConfirmedPhoneNumber = false;
                        options.SignIn.RequireConfirmedAccount = false;
                        options.Password.RequireNonAlphanumeric = false;
                        options.Password.RequireLowercase = false;
                        options.Password.RequireUppercase = false;
                        options.Password.RequiredLength = 1;
                        options.Password.RequireDigit = false;

                    })
                .AddUserManager<UserManager<ChessUser>>()
                .AddEntityFrameworkStores<ChessIdentityDbContext>()
                .AddUserStore<UserStore<ChessUser, ChessRole, ChessIdentityDbContext>>()
                .AddUserStore<UserStore<ChessUser, ChessRole, ChessIdentityDbContext>>()
                .AddRoleStore<RoleStore<ChessRole, ChessIdentityDbContext>>();
            builder.Services.AddSignalR();
            builder.Services.AddSingleton<IOfflineGameService, OfflineGameService>();
            builder.Services.AddSingleton<IOnlineGameService, OnlineGameService>();
            var app = builder.Build();
            app.UseRequestLocalization();
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.MapHub<OfflineGameHub>("/hubs/offlinegame");
            app.MapHub<OnlineGameHub>("/hubs/onlinegame");

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StackExchange.Redis;
using Talabat.APIs.Extentions;
using Talabat.APIs.Middlewares;
using Talabat.Core.Entities.Identity;
using Talabat.Repository.Data;
using Talabat.Repository.Identity;

namespace Talabat.APIs
{
    public class Program
    {
        public static async Task Main(string[] args)
        {

            var webApplicationBuilder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            #region Configure Services

            webApplicationBuilder.Services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });

            webApplicationBuilder.Services.AddSwaggerServices();

            webApplicationBuilder.Services.AddApplicationServices();

            webApplicationBuilder.Services.AddDbContext<StoreContext>(options =>
            {
                options.UseSqlServer(webApplicationBuilder.Configuration.GetConnectionString("DefaultConnection"));
            });

            webApplicationBuilder.Services.AddDbContext<AppIdentityDbContext>(options =>
            {
                options.UseSqlServer(webApplicationBuilder.Configuration.GetConnectionString("IdentityConnection"));
            });

            webApplicationBuilder.Services.AddSingleton<IConnectionMultiplexer>((serviceProvider) =>
            {
                var connectionString = webApplicationBuilder.Configuration.GetConnectionString("Redis");

                return ConnectionMultiplexer.Connect(connectionString);
            });

            webApplicationBuilder.Services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<AppIdentityDbContext>();

            webApplicationBuilder.Services.AddIdentityServices(webApplicationBuilder.Configuration);

            var frontBaseUrl = webApplicationBuilder.Configuration["FrontBaseUrl"];

            if (string.IsNullOrEmpty(frontBaseUrl))
            {
                throw new InvalidOperationException("FrontBaseUrl is not configured.");
            }

            webApplicationBuilder.Services.AddCors(options =>
            {
                options.AddPolicy("MyPolicy", option =>
                {
                    option.AllowAnyHeader();
                    option.AllowAnyMethod();
                    option.WithOrigins(frontBaseUrl);
                });
            });

            #endregion


            var app = webApplicationBuilder.Build();


            #region Apple All Pending Migrations and Data Seeding

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var _dbContext = services.GetRequiredService<StoreContext>();
                var _identityDbContext = services.GetRequiredService<AppIdentityDbContext>();
                var _userManager = services.GetRequiredService<UserManager<AppUser>>();

                var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger<Program>();

                try
                {
                    await _dbContext.Database.MigrateAsync(); // Update StoreContext Database
                    await StoreContextSeed.SeedAsync(_dbContext); // Seed StoreContext Data

                    await _identityDbContext.Database.MigrateAsync(); // Update AppIdentityDbContext Database
                    await AppIdentityDbContextSeed.SeedUserAsync(_userManager); // Seed AppIdentityDbContext Data
                }
                catch (DbUpdateException ex)
                {
                    logger.LogError(ex, "An error occurred while updating the database.");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred during data seeding or migration.");
                }
            }

            #endregion

            // Configure the HTTP request pipeline.
            #region Configure Kestrel Middlewares

            app.UseMiddleware<ExceptionMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwaggerMiddleware();
            }

            app.UseStatusCodePagesWithReExecute("/errors/{0}");

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseCors("MyPolicy");

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            #endregion

            app.Run();
        }
    }
}

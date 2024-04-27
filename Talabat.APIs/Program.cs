using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Talabat.APIs.Extentions;
using Talabat.APIs.Middlewares;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Services.Contract;
using Talabat.Repository.Data;
using Talabat.Repository.Identity;
using Talabat.Service.AuthService;

namespace Talabat.APIs
{
    public class Program
    {
        public static async Task Main(string[] args)
        {

            var webApplicationBuilder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            #region Configure Services

            webApplicationBuilder.Services.AddControllers();

            webApplicationBuilder.Services.AddSwaggerServices();

            webApplicationBuilder.Services.addApplicationServices(); 

            webApplicationBuilder.Services.AddDbContext<StoreContext>(options => options.UseSqlServer(webApplicationBuilder.Configuration.GetConnectionString("DefaultConnection")));

            webApplicationBuilder.Services.AddDbContext<AppIdentityDbContext>(options =>
            {
                options.UseSqlServer(webApplicationBuilder.Configuration.GetConnectionString("IdentityConnection"));
            });

            webApplicationBuilder.Services.AddSingleton<IConnectionMultiplexer>((serviceProvider) =>
            {
                var connection = webApplicationBuilder.Configuration.GetConnectionString("Redis");
                return ConnectionMultiplexer.Connect(connection);
            });

            webApplicationBuilder.Services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                ///options.Password.RequireDigit = true;
                ///options.Password.RequiredUniqueChars = 2;
                ///options.Password.RequireNonAlphanumeric = true;
            }).AddEntityFrameworkStores<AppIdentityDbContext>();

            webApplicationBuilder.Services.AddScoped(typeof(ITokenServices), typeof(TokenServices));


            //webApplicationBuilder.Services.AddIdentityServices();

            #endregion


            var app = webApplicationBuilder.Build();


            using var scope = app.Services.CreateScope();

            var services = scope.ServiceProvider;

            var _dbContext = services.GetRequiredService<StoreContext>();
            var _identityDbContext = services.GetRequiredService<AppIdentityDbContext>();

            var loggerFactory = services.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger<Program>();

            try
            {
                await _dbContext.Database.MigrateAsync(); // Update Database

                await StoreContextSeed.SeedAsync(_dbContext); // Data Seeding

                await _identityDbContext.Database.MigrateAsync();

                var _userManger = services.GetRequiredService<UserManager<AppUser>>();

                await AppIdentityDbContextSeed.SeedUserAsync(_userManger);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An Error has been occured during apply the migration");
            }

            // Configure the HTTP request pipeline.
            #region Configure Kestrel Middlewares

            app.UseMiddleware<ExceptionMiddleware>();

            ///app.Use(async (httpContext, _next) =>
            ///{
            ///    try
            ///    {
            ///        // Take An Action With the request
            ///        await _next.Invoke(httpContext);
            ///        // Take An Action With the response
            ///    }
            ///    catch (Exception ex)
            ///    {
            ///        logger.LogError(ex.Message); // Development
            ///        httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            ///        httpContext.Response.ContentType = "application/json";
            ///        var response = app.Environment.IsDevelopment() ?
            ///            new ApiExceptionResponse((int)HttpStatusCode.InternalServerError, ex.Message, ex.StackTrace.ToString())
            ///            :
            ///            new ApiExceptionResponse((int)HttpStatusCode.InternalServerError);
            ///        var options = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            ///        var json = JsonSerializer.Serialize(response, options);
            ///        await httpContext.Response.WriteAsync(json);
            ///    }
            ///});

            if (app.Environment.IsDevelopment())
            {
                app.UseSwaggerMiddleware();
            }

            app.UseStatusCodePagesWithReExecute("/errors/{0}");

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            #endregion

            app.Run();
        }
    }
}

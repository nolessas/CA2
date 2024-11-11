using CA1.Interface;
using CA1.Service;
using CA1.Database;
using Microsoft.EntityFrameworkCore;

namespace CA1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            ConfigureServices(builder.Services, builder.Configuration);

            var app = builder.Build();
            ConfigureApp(app);

            app.Run();
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Add services
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IMyLogger, MyLogger>();
            services.AddSingleton<LogRecordMapper>();

            // Database
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // API essentials
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }

        private static void ConfigureApp(WebApplication app)
        {
            // Initialize database
            using (var scope = app.Services.CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.EnsureCreated();
            }

            // Configure middleware
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
        }
    }
}

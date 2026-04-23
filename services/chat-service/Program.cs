using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Repositories;
using WebApplication1.Services;

namespace WebApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure Database
            builder.Services.AddDbContext<WebApplication1Context>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("WebApplication1Context")
                    ?? throw new InvalidOperationException("Connection string 'WebApplication1Context' not found.")));

            // Register Dependencies
            builder.Services.AddScoped<IChatRepository, ChatRepository>();
            builder.Services.AddScoped<IChatService, ChatService>();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Apply migrations automatically
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<WebApplication1Context>();
                db.Database.Migrate();
                // Note: The manual User seeding was removed from here. 
                // It is now safely handled in WebApplication1Context.cs via HasData()
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using WebApplication1.Data;
using WebApplication1.Mappings;
using WebApplication1.Middleware;
using WebApplication1.Repositories;
using WebApplication1.Services;

namespace WebApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File("Logs/API_Log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            builder.Host.UseSerilog();

            builder.Services.AddControllers();

            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssemblyContaining<Program>();

            builder.Services.AddAutoMapper(typeof(MappingProfile));

            builder.Services.AddDbContext<WebApplication1Context>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("WebApplication1Context")));

            builder.Services.AddScoped<IChatService, ChatService>();
            builder.Services.AddScoped<IChatRepository, ChatRepository>();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.UseSerilogRequestLogging();
            app.UseMiddleware<ExceptionMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                    c.RoutePrefix = string.Empty;
                });
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using System.Threading.RateLimiting;
using Yarp.ReverseProxy;

namespace GatewayAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // 🔍 Logging (Serilog)
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("logs/gateway-log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog();

            // 🔐 JWT Authentication
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = "yourIssuer",
                        ValidAudience = "yourAudience",
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes("your_super_secret_key_12345"))
                    };
                });

            builder.Services.AddAuthorization();

            // 🚦 Rate Limiting
            builder.Services.AddRateLimiter(options =>
            {
                options.AddFixedWindowLimiter("fixed", opt =>
                {
                    opt.PermitLimit = 5; // 5 requests
                    opt.Window = TimeSpan.FromSeconds(10);
                    opt.QueueLimit = 2;
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                });
            });

            // 🌐 YARP Reverse Proxy
            builder.Services.AddReverseProxy()
                .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

            var app = builder.Build();

            // 🔍 Logging middleware
            app.UseSerilogRequestLogging();

            // 🔐 Auth middleware
            app.UseAuthentication();
            app.UseAuthorization();

            // 🚦 Rate limiting middleware
            app.UseRateLimiter();

            // Test route
            app.MapGet("/", () => "Gateway running with Auth + Rate Limit");

            // 🌐 Proxy routes
            app.MapReverseProxy();

            app.Run();
        }
    }
}

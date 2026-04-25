using LearningPlatform.StudentService.Data;
using LearningPlatform.StudentService.Services;
using LearningPlatform.StudentService.Repositories;
using LearningPlatform.StudentService.Validators;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Extensions.Http;
using LearningPlatform.StudentService.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;


namespace LearningPlatform.StudentService
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var retryPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(1, retryAttempt => TimeSpan.FromSeconds(1));
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddControllers();
            
            // FluentValidation Registration
            builder.Services.AddValidatorsFromAssemblyContaining<BookmarkDtoValidator>();
            builder.Services.AddFluentValidationAutoValidation();
            
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Enter token like: Bearer {your_token}"
                });

                options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            builder.Services.AddHttpClient<IBookDiscoveryService, BookDiscoveryService>()
                .AddPolicyHandler(retryPolicy);

            builder.Services.AddHttpClient("CourseService", client =>
            {
                client.BaseAddress = new Uri(
                    builder.Configuration["ServiceUrls:CourseService"] ?? "https://localhost:7081/");
                client.Timeout = TimeSpan.FromSeconds(3);
            }).AddPolicyHandler(retryPolicy);

            builder.Services.AddHttpClient("ChatService", client =>
            {
                client.BaseAddress = new Uri(
                    builder.Configuration["ServiceUrls:ChatService"] ?? "https://localhost:7082/");
            }).AddPolicyHandler(retryPolicy);

            builder.Services.AddScoped<IChatIntegrationService, ChatIntegrationService>();
            builder.Services.AddScoped<IDashboardService, DashboardService>();
            builder.Services.AddScoped<ICourseIntegrationService, CourseIntegrationService>();
            builder.Services.AddScoped<IBookmarkService, BookmarkService>();
            builder.Services.AddScoped<IProfileService, ProfileService>();
            builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
            builder.Services.AddScoped<IProgressRepository, ProgressRepository>();
            builder.Services.AddScoped<IBookmarkRepository, BookmarkRepository>();
            builder.Services.AddScoped<IProfileRepository, ProfileRepository>();
            builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();

            builder.Services.AddScoped<IRecommendationService, RecommendationService>();
            builder.Services.AddScoped<IStudentExperienceService, StudentExperienceService>();

            builder.Services.AddDbContext<StudentDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddMemoryCache();

            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();

            var jwtSettings = builder.Configuration.GetSection("Jwt");
            var jwtKey = jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key is not configured. Set Jwt:Key in appsettings or environment variables.");

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtKey))
                };
            });


            var app = builder.Build();

            // Seed database with test data
            await DatabaseSeeder.SeedAsync(app.Services);

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }


            app.UseHttpsRedirection();

            app.UseCors(policy =>
                policy.AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowAnyOrigin()); // TODO: restrict origins in production

            app.UseMiddleware<ErrorMiddleware>();

            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}

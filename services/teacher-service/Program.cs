using FluentValidation;
using FluentValidation.AspNetCore;
using LearningPlatform.TeacherService.Middleware;
using LearningPlatform.TeacherService.Services;
using LearningPlatform.TeacherService.Validators;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Polly;
using Polly.Extensions.Http;
using Serilog;
using System.Text;

namespace LearningPlatform.TeacherService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // ── Serilog bootstrap (same pattern as Student API) ──────────────
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("logs/teacher-service-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            try
            {
                Log.Information("Starting Teacher Service");
                CreateAndRun(args);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Teacher Service terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static void CreateAndRun(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Host.UseSerilog();

            var configuration = builder.Configuration;

            // ── Resilience policy (same as Student API) ──────────────────────
            var retryPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(2, retryAttempt =>
                    TimeSpan.FromMilliseconds(300 * retryAttempt));

            // ── HttpClients ──────────────────────────────────────────────────

            // Course API – Teacher API's only data dependency
            builder.Services.AddHttpClient("CourseApi", client =>
            {
                client.BaseAddress = new Uri(
                    configuration["ServiceUrls:CourseApi"] ?? "https://localhost:7003/");
                client.Timeout = TimeSpan.FromSeconds(10);
            }).AddPolicyHandler(retryPolicy);

            // Chat API – secondary integration point
            builder.Services.AddHttpClient("ChatApi", client =>
            {
                client.BaseAddress = new Uri(
                    configuration["ServiceUrls:ChatApi"] ?? "https://localhost:7004/");
                client.Timeout = TimeSpan.FromSeconds(5);
            }).AddPolicyHandler(retryPolicy);

            // ── Core infrastructure ──────────────────────────────────────────
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddControllers();

            // FluentValidation
            builder.Services.AddValidatorsFromAssemblyContaining<CourseWriteDtoValidator>();
            builder.Services.AddFluentValidationAutoValidation();

            // ── Application services (DI) ────────────────────────────────────
            builder.Services.AddScoped<IUserContext, UserContext>();
            builder.Services.AddScoped<ICourseApiClient, CourseApiClient>();
            builder.Services.AddScoped<ITeacherCourseService, TeacherCourseService>();
            builder.Services.AddScoped<ITeacherDashboardService, TeacherDashboardService>();
            builder.Services.AddScoped<IChatIntegrationService, ChatIntegrationService>();

            // ── JWT Authentication ───────────────────────────────────────────
            // Uses the same key / issuer / audience as the identity-service and
            // student-service so tokens issued by the auth service are accepted here.
            var jwtSection = configuration.GetSection("Jwt");
            var jwtKey = jwtSection["Key"]
                ?? throw new InvalidOperationException(
                    "Jwt:Key is not configured. Add it to appsettings.json or environment variables.");

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
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSection["Issuer"],
                    ValidAudience = jwtSection["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                                                  Encoding.UTF8.GetBytes(jwtKey))
                };
            });

            builder.Services.AddAuthorization();

            // ── Swagger / OpenAPI ────────────────────────────────────────────
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Learner Hub – Teacher API",
                    Version = "v1",
                    Description = "Manages teacher dashboard, course CRUD and chat integration."
                });

                // Show Bearer token input in Swagger UI
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter your JWT token (without the 'Bearer ' prefix)"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id   = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            // ── CORS ─────────────────────────────────────────────────────────
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
                // TODO: tighten origins in production
            });

            // ────────────────────────────────────────────────────────────────
            var app = builder.Build();
            // ────────────────────────────────────────────────────────────────

            // ── Middleware pipeline (order matters) ──────────────────────────
            app.UseSerilogRequestLogging();

            // Global exception → JSON ApiResponseDto<object>
            app.UseMiddleware<ExceptionMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Teacher API v1");
                    c.RoutePrefix = "swagger";
                });
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowAll");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
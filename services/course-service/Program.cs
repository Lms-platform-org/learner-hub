using AutoMapper;
using Courses.Api.Data;
using Courses.Api.Middlewares;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.Filters.Add<Courses.Api.Filters.ResponseWrapperFilter>();
});
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Learning Platform API", Version = "v1" });
    
});


// AutoMapper
builder.Services.AddAutoMapper(cfg => {
    cfg.AddProfile<Courses.Api.Mappings.MappingProfile>();
});

// FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories
builder.Services.AddScoped<Courses.Api.Repositories.ICourseRepository, Courses.Api.Repositories.CourseRepository>();
builder.Services.AddScoped<Courses.Api.Repositories.ICommentRepository, Courses.Api.Repositories.CommentRepository>();
builder.Services.AddScoped<Courses.Api.Repositories.IEnrollmentRepository, Courses.Api.Repositories.EnrollmentRepository>();

// Services
builder.Services.AddScoped<Courses.Api.Services.ICourseService, Courses.Api.Services.CourseService>();
builder.Services.AddScoped<Courses.Api.Services.ICommentService, Courses.Api.Services.CommentService>();
builder.Services.AddScoped<Courses.Api.Services.IEnrollmentService, Courses.Api.Services.EnrollmentService>();

// User Context and HttpContextAccessor
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<Courses.Api.Services.IUserContext, Courses.Api.Services.UserContext>();

// Authentication & Authorization (Mocked via middleware)
builder.Services.AddAuthentication("MockScheme")
    .AddCookie("MockScheme"); // Add a dummy scheme to satisfy ASP.NET Core requirements
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var jwtSection = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
                Encoding.UTF8.GetBytes(jwtSection["Key"]!))
        };
    });
//builder.Services.AddAuthorization();
builder.Services.AddAuthorization();


// Add CORS if MVC project needs to call it from browser
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

// Global Exception Handling
app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();
app.UseCors("AllowAll");

// Use Mock Auth in development to skip login
app.UseAuthentication();   // ← real JWT runs first
app.UseAuthorization();

if (app.Environment.IsDevelopment())
    app.UseMiddleware<MockAuthMiddleware>(); // ← only fires if JWT didn't authenticate

app.MapControllers();

app.Run();

using Microsoft.EntityFrameworkCore;
using Courses.Api.Data;
using FluentValidation.AspNetCore;
using FluentValidation;
using Courses.Api.Middlewares;
using Serilog;
using AutoMapper;
using Microsoft.OpenApi;


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
if (app.Environment.IsDevelopment())
{
    app.UseMiddleware<MockAuthMiddleware>();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

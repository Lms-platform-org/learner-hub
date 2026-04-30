using LearningPlatform.StudentService.WebApp.Services;
namespace LearningPlatform.StudentService.WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllersWithViews();
            builder.Services.AddSession();
            builder.Services.AddHttpContextAccessor();
            

            builder.Services.AddHttpClient("StudentAPI", client =>
            {
                client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7090/");
                client.Timeout = TimeSpan.FromSeconds(5);
            });

           

            builder.Services.AddScoped<BookmarkApiService>();
            builder.Services.AddScoped<DashboardApiService>();
            builder.Services.AddScoped<RecommendationApiService>();
            builder.Services.AddScoped<ProgressApiService>();
            builder.Services.AddScoped<ProfileApiService>();
            builder.Services.AddScoped<DiscoveryApiService>();
            builder.Services.AddScoped<EnrollmentApiService>();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // No redirect to login needed as auth is handled externally
            app.Use(async (context, next) =>
            {
                await next();
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseSession();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Dashboard}/{action=Index}/{id?}");

            app.Run();
        }
    }
}

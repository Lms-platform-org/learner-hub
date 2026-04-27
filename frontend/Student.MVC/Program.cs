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
                client.BaseAddress = new Uri("https://localhost:7090/");
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

            // redirect to login on 401
            app.Use(async (context, next) =>
            {
                try
                {
                    await next();
                    if (context.Response.StatusCode == 401)
                        context.Response.Redirect("/Login/Index");
                }
                catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    context.Response.Redirect("/Login/Index");
                }
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseSession();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Login}/{action=Index}/{id?}");

            app.Run();
        }
    }
}

using Microsoft.EntityFrameworkCore;

namespace Chat_MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();

            var apiBaseUrl = builder.Configuration["ApiBaseUrl"];

            builder.Services.AddHttpClient("ChatApi", client =>
            {
                client.BaseAddress = new Uri("https://localhost:7001");
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback =
                        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                };
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            app.MapGet("/", context =>
            {
                context.Response.Redirect("/chat");
                return Task.CompletedTask;
            });

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=ChatMvc}/{action=Index}/{id?}");

            app.MapControllers();  // Routes to ChatController

            app.Run();
        }
    }
}

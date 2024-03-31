using LetsTalkRaceApi.Data;
using LetsTalkRaceApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace LetsTalkRaceApi;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        // Configure PostgreSQL connection
        var connectionString = Configuration.GetConnectionString("PostgresConnection");
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false; // Adjust this as needed
            })
            .AddEntityFrameworkStores<ApplicationDbContext>();

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });
        });
        
        services.AddControllersWithViews();
        services.AddRazorPages();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Other middleware configurations...

        app.UseRouting();
        app.UseSwagger();
        app.UseSwaggerUI();
        
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            endpoints.MapRazorPages();
        });
    }
}
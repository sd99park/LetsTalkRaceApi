using System.Text;
using LetsTalkRaceApi.Data;
using LetsTalkRaceApi.Implementations;
using LetsTalkRaceApi.Interfaces;
using LetsTalkRaceApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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

        services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false; // Adjust this as needed
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddRoles<IdentityRole>();

        services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Scheme = "bearer"
            });
            c.OperationFilter<AuthenticationRequirementsOperationFilter>();
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
        });

        services.Configure<MailSettings>(Configuration.GetSection("MailSettings"));
        services.AddTransient<IMailService, MailService>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
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
            
            endpoints.MapControllerRoute(
                name: "login",
                pattern: "api/login/v1/{action=Index}/{id?}",
                defaults: new { controller = "Login" });
            
            endpoints.MapRazorPages();
        });
    }
}
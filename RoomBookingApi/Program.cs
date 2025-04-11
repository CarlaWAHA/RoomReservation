using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using RoomBookingApi.Data;
using RoomBookingApi.Models;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueApp",
        builder => builder
            .WithOrigins("http://localhost:5173")
            .WithOrigins("http://localhost:5174")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .SetIsOriginAllowed(origin => true)); 
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
   options.UseOpenIddict();
   options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddOpenIddict()
    .AddCore(options => options.UseEntityFrameworkCore().UseDbContext<ApplicationDbContext>())
    .AddServer(options =>
    {
        options.AllowPasswordFlow();
        options.SetTokenEndpointUris("/connect/token");
        options.AddDevelopmentEncryptionCertificate();
        options.AddDevelopmentSigningCertificate();
        options.UseAspNetCore().EnableTokenEndpointPassthrough();
    });

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AllowVueApp");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

await InitializeDatabase(app);

app.MapGet("/", () => "✅ RoomBooking API is up and running!");

app.Run();

async Task InitializeDatabase(WebApplication app)
{
    // IMPORTANT: D'abord créer la base de données et les tables
    using (var initializationScope = app.Services.CreateScope())
    {
        var services = initializationScope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            
            // Utiliser Migrate au lieu de EnsureCreated pour appliquer les migrations
            // qui créeront toutes les tables Identity correctement
            context.Database.Migrate();
            // OU si vous n'utilisez pas les migrations:
            // context.Database.EnsureCreated();
            
            Console.WriteLine("Base de données créée avec succès");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur de création de la base de données : {ex.Message}");
            // Terminer l'exécution pour éviter des erreurs en cascade
            return;
        }
    }

    // ENSUITE seulement créer les rôles et l'administrateur
    using (var scope = app.Services.CreateScope())
    {
        try {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Vérifier si les rôles existent déjà avant de les créer
            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new IdentityRole("Admin"));
                
            if (!await roleManager.RoleExistsAsync("User"))
                await roleManager.CreateAsync(new IdentityRole("User"));

            // Vérifier si l'admin existe déjà
            var adminUser = await userManager.FindByEmailAsync("admin@example.com");
            if (adminUser == null)
            {
                var admin = new ApplicationUser { UserName = "admin@example.com", Email = "admin@example.com" };
                var result = await userManager.CreateAsync(admin, "Admin@123");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                    Console.WriteLine("Compte administrateur créé avec succès");
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    Console.WriteLine($"Erreur lors de la création de l'administrateur: {errors}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de l'initialisation des rôles et utilisateurs: {ex.Message}");
        }
    }
}

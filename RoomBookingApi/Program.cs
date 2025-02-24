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

// 🔹 Ajouter les services au conteneur
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 🔹 Configuration CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .SetIsOriginAllowed(origin => true);
    });
});

// 🔹 Configurer SQLite et OpenIddict
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.UseOpenIddict();
});

// 🔹 Ajout de Identity pour l'authentification
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// 🔹 Configuration OpenIDdict
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

// 🔹 Ajouter JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();

builder.Services.AddAuthorization();

var app = builder.Build();

// 🔹 Activer l'authentification et autorisation
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AllowVueApp");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

// 🔹 Création d’un administrateur au démarrage
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    await roleManager.CreateAsync(new IdentityRole("Admin"));
    await roleManager.CreateAsync(new IdentityRole("User"));

    var admin = new ApplicationUser { UserName = "admin@example.com", Email = "admin@example.com" };
    var result = await userManager.CreateAsync(admin, "Admin@123");

    if (result.Succeeded)
    {
        await userManager.AddToRoleAsync(admin, "Admin");
    }
}

// 🔹 Vérifier et créer la base de données
using (var initializationScope = app.Services.CreateScope())
{
    var services = initializationScope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.EnsureCreated();
        Console.WriteLine("Base de données créée avec succès");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erreur de création de la base de données : {ex.Message}");
    }
}

app.Run();

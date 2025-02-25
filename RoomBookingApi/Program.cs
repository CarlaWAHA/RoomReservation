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
    options.AddPolicy("AllowVueApp",
        builder => builder
            .WithOrigins("http://localhost:5173")
            .WithOrigins("http://localhost:5174")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .SetIsOriginAllowed(origin => true)); // Permet toutes les origines en développement
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

// Endpoints pour les réservations
app.MapGet("/api/reservations", async (ApplicationDbContext db) =>
{
    try
    {
        Console.WriteLine($"[{DateTime.Now}] Récupération des réservations");
        var reservations = await db.Reservations
            .Include(r => r.Room)
            .ToListAsync();
        Console.WriteLine($"[{DateTime.Now}] {reservations.Count} réservations trouvées");
        return Results.Ok(reservations);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[{DateTime.Now}] Erreur: {ex.Message}");
        return Results.Problem(ex.Message);
    }
});

app.MapPost("/api/reservations", async (ApplicationDbContext db, Reservation reservation) =>
{
    try
    {
        Console.WriteLine($"[{DateTime.Now}] Nouvelle réservation reçue: {reservation.Title}");
        // Validation des données
        if (string.IsNullOrEmpty(reservation.Title))
            return Results.BadRequest(new { message = "Le titre est requis" });

        if (reservation.RoomId <= 0)
            return Results.BadRequest(new { message = "L'ID de la salle est invalide" });

        // Vérifier si la salle existe
        var room = await db.Rooms.FindAsync(reservation.RoomId);
        if (room == null)
            return Results.BadRequest(new { message = "La salle spécifiée n'existe pas" });

        // Créer la réservation
        var newReservation = new Reservation
        {
            Title = reservation.Title,
            Date = reservation.Date,
            Start = reservation.Start,
            End = reservation.End,
            Description = reservation.Description,
            RoomId = reservation.RoomId
        };

        db.Reservations.Add(newReservation);
        await db.SaveChangesAsync();
        Console.WriteLine($"[{DateTime.Now}] Réservation sauvegardée avec l'ID: {newReservation.Id}");
        
        // Charger la salle associée
        await db.Entry(newReservation).Reference(r => r.Room).LoadAsync();
        
        return Results.Created($"/api/reservations/{newReservation.Id}", newReservation);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[{DateTime.Now}] Erreur: {ex.Message}");
        return Results.Problem(ex.Message);
    }
});

app.MapPut("/api/reservations/{id}", async (ApplicationDbContext db, int id, Reservation reservation) =>
{
    var existingReservation = await db.Reservations.FindAsync(id);
    if (existingReservation == null) return Results.NotFound();

    existingReservation.Title = reservation.Title;
    existingReservation.Date = reservation.Date;
    existingReservation.Start = reservation.Start;
    existingReservation.End = reservation.End;
    existingReservation.Description = reservation.Description;
    existingReservation.RoomId = reservation.RoomId;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/api/reservations/{id}", async (ApplicationDbContext db, int id) =>
{
    var reservation = await db.Reservations.FindAsync(id);
    if (reservation == null) return Results.NotFound();

    db.Reservations.Remove(reservation);
    await db.SaveChangesAsync();
    return Results.Ok();
});

// Assurez-vous que la base de données est créée au démarrage
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
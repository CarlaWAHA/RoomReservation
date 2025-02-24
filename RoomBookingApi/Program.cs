using Microsoft.EntityFrameworkCore;
using RoomBookingApi.Data;
using RoomBookingApi.Models;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Ajouter CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueApp",
        builder => builder
            .WithOrigins("http://localhost:5173")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .SetIsOriginAllowed(origin => true)); // Permet toutes les origines en développement
});

// Configurer SQLite
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// IMPORTANT: Mettre UseCors avant les autres middleware
app.UseCors("AllowVueApp");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Endpoints pour les salles
app.MapGet("/api/rooms", async (ApplicationDbContext db) =>
    await db.Rooms.ToListAsync());

// Endpoints pour les réservations
app.MapGet("/api/reservations", async (ApplicationDbContext db) =>
{
    try
    {
        var reservations = await db.Reservations
            .Include(r => r.Room)
            .ToListAsync();
        return Results.Ok(reservations);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erreur lors de la récupération des réservations: {ex.Message}");
        return Results.Problem(ex.Message);
    }
});

app.MapPost("/api/reservations", async (ApplicationDbContext db, Reservation reservation) =>
{
    try
    {
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
        
        // Charger la salle associée
        await db.Entry(newReservation).Reference(r => r.Room).LoadAsync();
        
        return Results.Created($"/api/reservations/{newReservation.Id}", newReservation);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erreur lors de la création de la réservation: {ex.Message}");
        return Results.BadRequest(new { message = ex.Message });
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
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.EnsureCreated();
        Console.WriteLine("Base de données créée avec succès");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Une erreur s'est produite lors de la création de la base de données : {ex.Message}");
    }
}

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomBookingApi.Data;
using RoomBookingApi.Models;

namespace RoomBookingApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ReservationsController> _logger;

        public ReservationsController(ApplicationDbContext context, ILogger<ReservationsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                _logger.LogInformation("Récupération des réservations");
                var reservations = await _context.Reservations
                    .Include(r => r.Room)
                    .ToListAsync();
                _logger.LogInformation("{Count} réservations trouvées", reservations.Count);
                return Ok(reservations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des réservations");
                return Problem(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var reservation = await _context.Reservations
                    .Include(r => r.Room)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (reservation == null)
                    return NotFound();

                return Ok(reservation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de la réservation {Id}", id);
                return Problem(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(Reservation reservation)
        {
            try
            {
                _logger.LogInformation("Nouvelle réservation reçue: {Title}", reservation.Title);
                
                // Validation des données
                if (string.IsNullOrEmpty(reservation.Title))
                    return BadRequest(new { message = "Le titre est requis" });

                if (reservation.RoomId <= 0)
                    return BadRequest(new { message = "L'ID de la salle est invalide" });

                // Vérifier si la salle existe
                var room = await _context.Rooms.FindAsync(reservation.RoomId);
                if (room == null)
                    return BadRequest(new { message = "La salle spécifiée n'existe pas" });

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

                _context.Reservations.Add(newReservation);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Réservation sauvegardée avec l'ID: {Id}", newReservation.Id);
                
                // Charger la salle associée
                await _context.Entry(newReservation).Reference(r => r.Room).LoadAsync();
                
                return CreatedAtAction(nameof(GetById), new { id = newReservation.Id }, newReservation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création de la réservation");
                return Problem(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Reservation reservation)
        {
            if (id != reservation.Id)
                return BadRequest("L'ID de la réservation ne correspond pas");

            var existingReservation = await _context.Reservations.FindAsync(id);
            if (existingReservation == null) 
                return NotFound();

            existingReservation.Title = reservation.Title;
            existingReservation.Date = reservation.Date;
            existingReservation.Start = reservation.Start;
            existingReservation.End = reservation.End;
            existingReservation.Description = reservation.Description;
            existingReservation.RoomId = reservation.RoomId;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Erreur de concurrence lors de la mise à jour de la réservation {Id}", id);
                return Problem("Erreur de concurrence lors de la mise à jour");
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null) 
                return NotFound();

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();
            
            return Ok();
        }
    }
} 

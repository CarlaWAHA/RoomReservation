namespace RoomBookingApi.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
        public string Description { get; set; }
        public int RoomId { get; set; }
        public Room Room { get; set; }
    }
} 

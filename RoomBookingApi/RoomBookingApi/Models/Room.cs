using System.ComponentModel.DataAnnotations;

namespace RoomBookingApi.Models
{
    public class Room
    {
        public int Id { get; set; }
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;
        [Range(0, 1000)]
        public int Capacity { get; set; }
        
         [Range(0, 10000)]
         public decimal Area { get; set; }
        public bool IsAccessible { get; set; }
        public override string ToString(){
            return Name;
        }
       // public string ExtendsName =>$Name ;
    }
}
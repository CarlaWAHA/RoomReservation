using Microsoft.AspNetCore.SignalR;
using Microsoft.Net.Http.Headers;

namespace RoomBookingApi.Models
{

    public class ApplicationSettings
    {
        public string? ApiName { get; set; }
        public string? Version { get; set; }
    }
}
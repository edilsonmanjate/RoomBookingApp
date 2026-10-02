namespace RoomBookingApp.Core.Models
{
    public class RoomBookingRequest
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateOnly Date { get; set; }
    }
}
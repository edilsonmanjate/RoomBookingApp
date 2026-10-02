using RoomBookingApp.Core.Services;
using RoomBookingApp.Domain;

namespace RoomBookingApp.Persistence.Respositories;

public class RommBookingService : IRoomBookingService
{
    private readonly RoomBookingDbContext _context;

    public RommBookingService(RoomBookingDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Room> GetAvailableRooms(DateTime date)
    {
        return _context.Rooms.Where(r => !r.RoomBookings.Any(rb =>rb.Date == date)).ToList();
    }

    public void Save(RoomBooking roomBooking)
    {
        _context.RoomBookings.Add(roomBooking);
        _context.SaveChanges();
    }
}

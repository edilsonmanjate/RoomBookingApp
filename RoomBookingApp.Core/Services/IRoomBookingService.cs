using RoomBookingApp.Core.Domain;

namespace RoomBookingApp.Core.Services;

public interface IRoomBookingService
{
    void save(RoomBooking roomBooking);

    IEnumerable<Rooms> GetAvailableRooms(DateTime date);

}

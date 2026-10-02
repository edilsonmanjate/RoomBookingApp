using Microsoft.EntityFrameworkCore;

using RoomBookingApp.Domain;
using RoomBookingApp.Persistence.Respositories;

namespace RoomBookingApp.Persistence.Tests;

public class RoomBookingServiceTest
{
    [Fact]
    public void Should_Return_Available_Rooms()
    {
        // Arrange
        var date = new DateTime(2024, 6, 1);

        var dbOptions = new DbContextOptionsBuilder<RoomBookingDbContext>()
            .UseInMemoryDatabase(databaseName: "RoomBookingDb")
            .Options;

        var context = new RoomBookingDbContext(dbOptions);

        context.Add( new Room { Id = 1, Name = "Room 1" });
        context.Add( new Room { Id = 2, Name = "Room 2" });
        context.Add( new Room { Id = 3, Name = "Room 3" });

        context.Add(new RoomBooking { RoomId = 1, Date = date });
        context.Add(new RoomBooking { RoomId = 2, Date = date.AddDays(-1) });

        context.SaveChanges();

        var roomBookingservice = new RommBookingService(context);

        // Act
        var availableRooms = roomBookingservice.GetAvailableRooms(date);

        // Assert
        Assert.Equal(2, availableRooms.Count());
        Assert.Contains(availableRooms, r => r.Id == 2);
        Assert.Contains(availableRooms, r => r.Id == 3);
        Assert.DoesNotContain(availableRooms, r => r.Id == 1);
    }

    [Fact]
    public void Should_Save_RoomBooking()
    {
        // Arrange
        var date = new DateTime(2024, 6, 1);
        var dbOptions = new DbContextOptionsBuilder<RoomBookingDbContext>()
            .UseInMemoryDatabase(databaseName: "RoomBookingDb_SaveTest")
            .Options;


        var context = new RoomBookingDbContext(dbOptions);
        context.Add(new Room { Id = 1, Name = "Room 1" });
        context.SaveChanges();

        var roomBookingservice = new RommBookingService(context);

        // Act
        roomBookingservice.Save(new RoomBooking { RoomId = 1, Date = date });

        // Assert
        var savedBooking = context.RoomBookings.FirstOrDefault(rb => rb.RoomId == 1 && rb.Date == date);
        Assert.NotNull(savedBooking);
    }
}

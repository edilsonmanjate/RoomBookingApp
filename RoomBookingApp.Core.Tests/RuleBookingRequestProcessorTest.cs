using Moq;

using RoomBookingApp.Core.Domain;
using RoomBookingApp.Core.Enums;
using RoomBookingApp.Core.Models;
using RoomBookingApp.Core.Processors;
using RoomBookingApp.Core.Services;

using Shouldly;

namespace RoomBookingApp.Core;

public class RuleBookingRequestProcessorTest
{
    private RoomBookingRequestProcessor _processor;
    private RoomBookingRequest _bookingRequest;
    private Mock<IRoomBookingService> _roomBookingServiceMock;
    private List<Rooms> _availableRooms;

    public RuleBookingRequestProcessorTest()
    {
        // Arrange
        _bookingRequest = new RoomBookingRequest
        {
            FullName = "John Doe",
            Email = "john.doe@example.com",
            Date = new DateTime(2024, 6, 1)
        };
        _availableRooms = new List<Rooms>{ 
            new Rooms() {Id = 1, Name = "Room A"}
        };
        
        _roomBookingServiceMock = new Mock<IRoomBookingService>();
        _roomBookingServiceMock.Setup(r => r.GetAvailableRooms(_bookingRequest.Date)).Returns(_availableRooms);
        _processor = new RoomBookingRequestProcessor(_roomBookingServiceMock.Object);
    }

    [Fact]
    public void Should_Return_Room_Booking_Response_With_Requested_Values()
    {
        // Arrange
 
        // Act
        RoomBookingResult result = _processor.BookRoom(_bookingRequest);

        // Assert
        //Assert.NotNull(result);
        //Assert.Equal(bookingRequest.FullName, result.FullName);
        //Assert.Equal(bookingRequest.Email, result.Email);
        //Assert.Equal(bookingRequest.Date, result.Date);

        result.ShouldNotBeNull();
        result.FullName.ShouldBe(_bookingRequest.FullName);
        result.Email.ShouldBe(_bookingRequest.Email);
        result.Date.ShouldBe(_bookingRequest.Date);
    }

    [Fact]
    public void Should_Throw_Exception_When_Booking_Request_Is_Null()
    {
        // Act & Assert
        var exception = Should.Throw<ArgumentNullException>(() => _processor.BookRoom(null));

        exception.ParamName.ShouldBe("bookingRequest");
    }

    [Fact]
    public void Should_Save_Room_Booking_Request()
    {
        // Act
        RoomBooking savedBooking = null;
        _roomBookingServiceMock.Setup(service => service.save(It.IsAny<RoomBooking>()))
                .Callback<RoomBooking>(booking => savedBooking = booking);

        _processor.BookRoom(_bookingRequest);

        // Assert
        _roomBookingServiceMock.Verify(service => service.save(It.IsAny<RoomBooking>()), Times.Once);

        savedBooking.ShouldNotBeNull();
        savedBooking.FullName.ShouldBe(_bookingRequest.FullName);
        savedBooking.Email.ShouldBe(_bookingRequest.Email);
        savedBooking.Date.ShouldBe(_bookingRequest.Date);
        savedBooking.RoomId.ShouldBe(_availableRooms.First().Id);
    }


    [Fact]
    public void Should_Not_Save_Room_Booking_Request_If_None_Available()
    {
        // Arrange
        _availableRooms.Clear(); // No available rooms

        // Act
        RoomBookingResult result = _processor.BookRoom(_bookingRequest);

        // Assert
        _roomBookingServiceMock.Verify(service => service.save(It.IsAny<RoomBooking>()), Times.Never);
        result.ShouldNotBeNull();
        result.FullName.ShouldBe(_bookingRequest.FullName);
        result.Email.ShouldBe(_bookingRequest.Email);
        result.Date.ShouldBe(_bookingRequest.Date);

    }

    [Theory]
    [InlineData(BookingResultFlag.Success, true)]
    [InlineData(BookingResultFlag.Failure, false)]
    public void Should_Return_SuccessOrFailure_Flag_InResult(BookingResultFlag bookingSuccessFlag, bool isAvailable)
    {
        // Arrange
        if (!isAvailable)
        {
            _availableRooms.Clear();
        }

        // Act
        var result = _processor.BookRoom(_bookingRequest);

        // Assert
        bookingSuccessFlag.ShouldBe(result.Flag);
    }

    [Theory]
    [InlineData(1, true)]
    [InlineData(null, false)]
    public void Should_Return_Room_bookingId_InResult(int? roomBookingId, bool isAvailable)
    {
        if (!isAvailable)
        {
            _availableRooms.Clear();
        }
        else
        {
            _availableRooms[0].Id = roomBookingId.Value;

            RoomBooking savedBooking = null;
            _roomBookingServiceMock.Setup(service => service.save(It.IsAny<RoomBooking>()))
                    .Callback<RoomBooking>(booking => booking.RoomId = roomBookingId.Value);
        }

        var result = _processor.BookRoom(_bookingRequest);

        if (isAvailable)
        {
            result.RoomBookingId.ShouldBe(roomBookingId);
        }
        else
        {
            result.RoomBookingId.ShouldBeNull();
        }

    }

}

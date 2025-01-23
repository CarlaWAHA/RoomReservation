using Moq;

namespace RoomBookingApi.Tests;

public class RoomControllerTests
{
    [Fact]
    public void GetShouldReturnAllRooms()
    {
            //Arrange/Given
            var mockContext=new Mock<RoomApiContext>();
            mockContext.Setup<Dbset<Room>>(context=>context.Rooms);

            var mockLogger= new Mock<Ilogger<RoomController>>();

            var roomController=new RoomController(mockContext.Objet,            mockogger.Oject);

            //Act/When
            var rooms = roomControler.GetRooms();
            //Assert/Then
            Assert.Equal(3, rooms.value.Count());
    }
}
using System.Text.RegularExpressions;
using AdvancedREI;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using ProjectDeception.Backend.Hubs;
using ProjectDeception.Backend.Models;
using DbContext = ProjectDeception.Backend.Data.DbContext;

namespace ProjectDeception.Tests.Backend.Hubs
{
    /// <summary>
    /// Tests suite for <see cref="RoomsHub"/>
    /// </summary>
    [TestFixture]
    public partial class RoomsHubTests
    {
        // Regular expression for
        // testing generated room codes
        [GeneratedRegex("[A-Z, 0-9]{6}")]
        private static partial Regex RoomCodeRegex();

        // In-memory database for testing database operations
        private DbContext _dbContext;

        #region Set-up and Tear-down

        [OneTimeSetUp]
        public void OnetimeSetup()
        {
            DbContextOptions<DbContext> contextOptions = new DbContextOptionsBuilder<DbContext>()
                .UseInMemoryDatabase(databaseName: "MainDb")
                .Options;

            _dbContext = new DbContext(contextOptions);
        }

        [TearDown]
        public void TearDown()
        {
            // Reset database state
            _dbContext.RemoveRange(_dbContext.Rooms);
        }

        #endregion

        #region Client Request Tests

        [Test]
        public async Task RecieveCreateRoomRequest_EmptyDatabase()
        {
            // Arrange
            Mock<IHubCallerClients<IRoomsHubClient>> mockClients = new();
            Mock<IRoomsHubClient> mockClientProxy = new();

            mockClients.Setup(clients => clients.Caller)
                .Returns(mockClientProxy.Object);

            RoomsHub hub = new(_dbContext)
            {
                Clients = mockClients.Object
            };

            // Act
            string response = await hub.RecieveCreateRoomRequest();

            // Assert
            Assert.Multiple(() =>
            {
                // Ensure response is valid
                Assert.That(IsValidRoomCode(response), Is.True);

                // Ensure database state is valid
                Assert.That(_dbContext.Rooms.Count(), Is.EqualTo(1));
                // Ensure created room is as expected
                Room createdRoom = _dbContext.Rooms.First();
                Assert.That(createdRoom.Code, Is.EqualTo(response));
            });
        }

        [Test]
        public async Task RecieveCreateRoomRequest_PopulatedDatabase()
        {
            // Arrange
            Mock<IHubCallerClients<IRoomsHubClient>> mockClients = new();
            Mock<IRoomsHubClient> mockClientProxy = new();

            mockClients.Setup(clients => clients.Caller)
                .Returns(mockClientProxy.Object);

            RoomsHub hub = new(_dbContext)
            {
                Clients = mockClients.Object
            };

            _dbContext.Rooms.Add(new Room 
            { 
                Code = GenerateRoomCode(Random.Shared.Next(60466176, int.MaxValue)) 
            });
            _dbContext.Rooms.Add(new Room 
            { 
                Code = GenerateRoomCode(Random.Shared.Next(60466176, int.MaxValue)) 
            });
            _dbContext.Rooms.Add(new Room 
            { 
                Code = GenerateRoomCode(Random.Shared.Next(60466176, int.MaxValue)) 
            });
            await _dbContext.SaveChangesAsync();

            // Act
            string response = await hub.RecieveCreateRoomRequest();

            // Assert
            Assert.Multiple(() =>
            {
                // Ensure response is valid
                Assert.That(IsValidRoomCode(response), Is.True);

                // Ensure database state is valid
                Assert.That(_dbContext.Rooms.Count(), Is.EqualTo(4));
                // Ensure created room is as expected
                Room createdRoom = _dbContext.Rooms.First();
                Assert.That(createdRoom.Code, Is.EqualTo(response));
            });
        }

        #endregion

        #region Helper Functions

        private static bool IsValidRoomCode(string code)
        {
            return RoomCodeRegex().IsMatch(code);
        }

        private static string GenerateRoomCode(int seed)
        {
            // Ensure seed is within limits
            // (Room codes are actually a base 36 representation,
            // or [A-Z, 0-9]{6} as a regular expression, with the
            // minimum value being 100000 (or 60466176 in base10)
            // so it's guaranteed to return a code with six characters
            if (seed < 60466176 || seed > int.MaxValue || seed < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(seed));
            }

            return Base36.NumberToBase36(seed);
        }

        #endregion
    }
}

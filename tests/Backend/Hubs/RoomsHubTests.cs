using System.Text.RegularExpressions;
using AdvancedREI;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using ProjectDeception.Backend.Hubs;
using ProjectDeception.Backend.Models;
using ProjectDeception.Exceptions;
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
            _dbContext.RemoveRange(_dbContext.Players);
            _dbContext.SaveChanges();
        }

        #endregion

        #region Client Request Tests

        /// <summary>
        /// Unit test for <see cref="RoomsHub.RecieveCreateRoomRequest(string)"/>
        /// with no other rooms and players in the database
        /// </summary>
        /// <returns>A <see cref="Task"/> representing this action</returns>
        [Test]
        public async Task RecieveCreateRoomRequest_EmptyDatabase()
        {
            // Arrange
            Mock<IHubCallerClients<IRoomsHubClient>> mockClients    = new();
            Mock<IRoomsHubClient> mockClientProxy                   = new();
            Mock<HubCallerContext> mockHubCallerContext             = new();

            mockClients.Setup(clients => clients.Caller)
                .Returns(mockClientProxy.Object);
            mockHubCallerContext.Setup(hubCaller => hubCaller.ConnectionId)
                .Returns("333477a9d8fa46a0ae420d970b0dcd73");

            RoomsHub hub = new(_dbContext, _dbContext)
            {
                Clients = mockClients.Object,
                Context = mockHubCallerContext.Object
            };

            // Act
            string response = await hub.RecieveCreateRoomRequest("George");

            // Assert
            Assert.Multiple(() =>
            {
                Room? createdRoom        = _dbContext.Rooms.Find(response);
                Player? createdPlayer    = _dbContext.Players.Find("333477a9d8fa46a0ae420d970b0dcd73");

                // Ensure response is valid
                Assert.That(IsValidRoomCode(response), Is.True);

                // Ensure database state is valid
                Assert.That(_dbContext.Rooms.Count(), Is.EqualTo(1));
                // Ensure a user has been created for this request
                Assert.That(createdPlayer?.ConnectionId, Is.EqualTo("333477a9d8fa46a0ae420d970b0dcd73"));
                Assert.That(createdPlayer?.Name, Is.EqualTo("George"));
                Assert.That(createdPlayer?.Room, Is.EqualTo(createdRoom));
                // Ensure created room is as expected
                Assert.That(createdRoom?.Code, Is.EqualTo(response));
                Assert.That(createdRoom?.Players, Contains.Item(createdPlayer));
            });
        }

        /// <summary>
        /// Unit test for <see cref="RoomsHub.RecieveCreateRoomRequest(string)"/>
        /// with other rooms and players in the database
        /// </summary>
        /// <returns>A <see cref="Task"/> representing this action</returns>
        [Test]
        public async Task RecieveCreateRoomRequest_PopulatedDatabase()
        {
            // Arrange
            Mock<IHubCallerClients<IRoomsHubClient>> mockClients    = new();
            Mock<IRoomsHubClient> mockClientProxy                   = new();
            Mock<HubCallerContext> mockHubCallerContext             = new();

            mockClients.Setup(clients => clients.Caller)
                .Returns(mockClientProxy.Object);
            mockHubCallerContext.Setup(hubCaller => hubCaller.ConnectionId)
                .Returns("281f5d6f5b05437d967d96537db5c383");

            Room[] rooms            = new Room[] { new(), new(), new() };
            Player[] players        = new Player[] { new(), new(), new() };
            players[0].Room         = rooms[0];
            players[0].Name         = "Frank";
            players[0].ConnectionId = "fd79c73ea78d4ec39bb3d217fe7d66c2";
            rooms[0].Code           = GenerateRoomCode(60466176);
            rooms[0].Players        = new List<Player> { players[0] };
            players[1].Room         = rooms[1];
            players[1].Name         = "Arin";
            players[1].ConnectionId = "1674a16282aa4bf99c47bfe4761cad99";
            rooms[1].Code           = GenerateRoomCode(1375412751);
            rooms[1].Players        = new List<Player> { players[1] };
            players[2].Room         = rooms[1];
            players[2].Name         = "Anthony";
            players[2].ConnectionId = "02cb0fc388134b39b74f55957cc2d46f";
            rooms[2].Code           = GenerateRoomCode(1855852599);
            rooms[2].Players        = new List<Player> { players[2] };

            await _dbContext.AddRangeAsync(rooms);
            await _dbContext.AddRangeAsync(players);
            await _dbContext.SaveChangesAsync();

            RoomsHub hub = new(_dbContext, _dbContext)
            {
                Clients = mockClients.Object,
                Context = mockHubCallerContext.Object
            };

            // Act
            string response = await hub.RecieveCreateRoomRequest("Matthew");

            // Assert
            Assert.Multiple(() =>
            {
                Room? createdRoom        = _dbContext.Rooms.Find(response);
                Player? createdPlayer    = _dbContext.Players.Find("281f5d6f5b05437d967d96537db5c383");

                // Ensure response is valid
                Assert.That(IsValidRoomCode(response), Is.True);

                // Ensure database state is valid
                Assert.That(_dbContext.Rooms.Count(), Is.EqualTo(rooms.Length + 1));
                // Ensure a user has been created for this request
                Assert.That(createdPlayer?.ConnectionId, Is.EqualTo("281f5d6f5b05437d967d96537db5c383"));
                Assert.That(createdPlayer?.Name, Is.EqualTo("Matthew"));
                Assert.That(createdPlayer?.Room, Is.EqualTo(createdRoom));
                // Ensure created room is as expected
                Assert.That(createdRoom?.Code, Is.EqualTo(response));
                Assert.That(createdRoom?.Players, Contains.Item(createdPlayer));
            });
        }

        /// <summary>
        /// Unit test for <see cref="RoomsHub.RecieveJoinRoomRequest(string)"/>
        /// </summary>
        /// <returns>A <see cref="Task"/> representing this action</returns>
        [Test]
        public async Task RecieveJoinRoomRequest()
        {
            // Arrange
            Mock<IHubCallerClients<IRoomsHubClient>> mockClients    = new();
            Mock<IRoomsHubClient> mockClientProxy                   = new();
            Mock<HubCallerContext> mockHubCallerContext             = new();

            mockClients.Setup(clients => clients.Caller)
                .Returns(mockClientProxy.Object);
            mockHubCallerContext.Setup(hubCaller => hubCaller.ConnectionId)
                .Returns("30828d75df9f4008b266049ea0e87958");

            Room room           = new();
            Player player       = new();
            room.Code           = GenerateRoomCode(1340361989);
            room.Players        = new List<Player> { player };
            player.ConnectionId = "3b7bf21c2cb74141b2a10eb4cc1605b7";
            player.Name         = "Manny";
            player.Room         = room;

            await _dbContext.AddRangeAsync(player);
            await _dbContext.AddRangeAsync(room);
            await _dbContext.SaveChangesAsync();

            RoomsHub hub = new(_dbContext, _dbContext)
            {
                Clients = mockClients.Object,
                Context = mockHubCallerContext.Object
            };

            // Act
            await hub.RecieveJoinRoomRequest(room.Code, "Red");

            // Assert
            Assert.Multiple(() =>
            {
                Room? room               = _dbContext.Rooms.Find(GenerateRoomCode(1340361989));
                Player? createdPlayer    = _dbContext.Players.Find("30828d75df9f4008b266049ea0e87958");

                // Ensure database state is valid
                Assert.That(_dbContext.Players.Count(), Is.EqualTo(2));
                Assert.That(createdPlayer?.ConnectionId, Is.EqualTo("30828d75df9f4008b266049ea0e87958"));
                Assert.That(createdPlayer?.Name, Is.EqualTo("Red"));
                Assert.That(createdPlayer?.Room, Is.EqualTo(room));
                Assert.That(room?.Players, Is.Not.Null);
                Assert.That(room?.Players, Has.Count.EqualTo(2));
                Assert.That(room?.Players, Has.Member(createdPlayer));
            });
        }

        /// <summary>
        /// Unit test for <see cref="RoomsHub.RecieveJoinRoomRequest(string)"/>
        /// with an invalid <see cref="Room"/> code
        /// </summary>
        /// <returns>A <see cref="Task"/> representing this action</returns>
        [Test]
        public async Task RecieveJoinRoomRequest_InvalidRoomCode()
        {
            // Arrange
            Mock<IHubCallerClients<IRoomsHubClient>> mockClients = new();
            Mock<IRoomsHubClient> mockClientProxy = new();
            Mock<HubCallerContext> mockHubCallerContext = new();

            mockClients.Setup(clients => clients.Caller)
                .Returns(mockClientProxy.Object);
            mockHubCallerContext.Setup(hubCaller => hubCaller.ConnectionId)
                .Returns("1b56e914c9d54531b35b5fcd9a09d740");

            Room[] rooms            = new Room[] { new(), new() };
            Player[] players        = new Player[] { new(), new() };
            rooms[0].Code           = GenerateRoomCode(731028452);
            rooms[0].Players        = new List<Player> { players[0] };
            players[0].ConnectionId = "629efef350124c4f892128b3f2ecb7d5";
            players[0].Name         = "Neil";
            players[0].Room         = rooms[0];
            rooms[1].Code           = GenerateRoomCode(1836008191);
            rooms[1].Players        = new List<Player> { players[1] };
            players[1].ConnectionId = "3f73578a7cd74531bd90910e17f13fb1";
            players[1].Name         = "Rod";
            players[1].Room         = rooms[1];

            await _dbContext.AddRangeAsync(players);
            await _dbContext.AddRangeAsync(rooms);
            await _dbContext.SaveChangesAsync();

            RoomsHub hub = new(_dbContext, _dbContext)
            {
                Clients = mockClients.Object,
                Context = mockHubCallerContext.Object
            };

            // Act & Assert
            Assert.That(async () => await hub.RecieveJoinRoomRequest(GenerateRoomCode(1877670560), "Ben"),
                    Throws.TypeOf<InvalidRoomCodeException>());
        }

        /// <summary>
        /// Unit test for <see cref="RoomsHub.RecieveJoinRoomRequest(string)"/>
        /// when the <see cref="Player"/> is already in a <see cref="Room"/>
        /// </summary>
        /// <returns>A <see cref="Task"/> representing this action</returns>
        [Test]
        public async Task RecieveJoinRoomRequest_AlreadyInRoom()
        {
            // Arrange
            Mock<IHubCallerClients<IRoomsHubClient>> mockClients = new();
            Mock<IRoomsHubClient> mockClientProxy = new();
            Mock<HubCallerContext> mockHubCallerContext = new();

            mockClients.Setup(clients => clients.Caller)
                .Returns(mockClientProxy.Object);
            mockHubCallerContext.Setup(hubCaller => hubCaller.ConnectionId)
                .Returns("92e3e2b8f12b41579279a3efe11a5c45");

            Room[] rooms            = new Room[] { new(), new() };
            Player[] players        = new Player[] { new(), new(), new() };
            rooms[0].Code           = GenerateRoomCode(694078631);
            rooms[0].Players        = new List<Player> { players[0] };
            players[0].ConnectionId = "f15cdf99dd2b400bb09d4808899daba5";
            players[0].Name         = "Ray";
            players[0].Room         = rooms[0];
            rooms[1].Code           = GenerateRoomCode(979019483);
            rooms[1].Players        = new List<Player> { players[1] };
            players[1].ConnectionId = "b801f2148d744869b5cabaa6f6e03970";
            players[1].Name         = "Tod";
            players[1].Room         = rooms[1];
            players[2].ConnectionId = "92e3e2b8f12b41579279a3efe11a5c45";
            players[2].Name         = "Raven";
            players[2].Room         = rooms[0];

            await _dbContext.AddRangeAsync(players);
            await _dbContext.AddRangeAsync(rooms);
            await _dbContext.SaveChangesAsync();

            RoomsHub hub = new(_dbContext, _dbContext)
            {
                Clients = mockClients.Object,
                Context = mockHubCallerContext.Object
            };

            // Act & Assert
            Assert.That(async () =>
            {
#pragma warning disable CS8604 // Possible null reference argument.
                await hub.RecieveJoinRoomRequest(rooms[1].Code, "Brian");
#pragma warning restore CS8604 // Possible null reference argument.
            }, 
                Throws.TypeOf<AlreadyInRoomException>());
        }

        /// <summary>
        /// Unit test for <see cref="RoomsHub.RecieveLeaveRoomRequest"/>
        /// </summary>
        /// <returns>A <see cref="Task"/> representing this action</returns>
        [Test]
        public async Task RecieveLeaveRoomRequest()
        {
            // Arrange
            Mock<IHubCallerClients<IRoomsHubClient>> mockClients    = new();
            Mock<IRoomsHubClient> mockClientProxy                   = new();
            Mock<HubCallerContext> mockHubCallerContext             = new();

            mockClients.Setup(clients => clients.Caller)
                .Returns(mockClientProxy.Object);
            mockHubCallerContext.Setup(hubCaller => hubCaller.ConnectionId)
                .Returns("603323548ed44511bf192eec2e00e121");

            Player[] players        = new Player[] { new(), new() };
            Room room               = new();
            players[0].ConnectionId = "1f3c49a024854e27a4e1b86e3bcdfb10";
            players[0].Name         = "Ron";
            players[0].Room         = room;
            players[1].ConnectionId = "603323548ed44511bf192eec2e00e121";
            players[1].Name         = "Jake";
            players[1].Room         = room;
            room.Code               = GenerateRoomCode(1954610763);
            room.Players            = new List<Player> { players[0], players[1] };

            await _dbContext.AddRangeAsync(players);
            await _dbContext.AddAsync(room);
            await _dbContext.SaveChangesAsync();

            RoomsHub hub = new(_dbContext, _dbContext)
            {
                Clients = mockClients.Object,
                Context = mockHubCallerContext.Object
            };

            // Act
            await hub.RecieveLeaveRoomRequest();

            // Assert
            Assert.Multiple(() =>
            {
                Player? callingPlayer   = _dbContext.Players.Find("603323548ed44511bf192eec2e00e121");
                Room? roomInDatabase    = _dbContext.Rooms.Find(room.Code);

                Assert.That(callingPlayer, Is.Null);
                Assert.That(roomInDatabase?.Players, Has.Count.EqualTo(1));
                Assert.That(roomInDatabase?.Players, Has.Member(players[0]));
            });
        }

        /// <summary>
        /// Unit test for <see cref="RoomsHub.RecieveLeaveRoomRequest"/>
        /// when the last <see cref="Player"/> of the <see cref="Room"/>
        /// leaves
        /// </summary>
        /// <returns>A <see cref="Task"/> representing this action</returns>
        [Test]
        public async Task RecieveLeaveRoomRequest_LastPlayerInRoom()
        {
            // Arrange
            Mock<IHubCallerClients<IRoomsHubClient>> mockClients = new();
            Mock<IRoomsHubClient> mockClientProxy = new();
            Mock<HubCallerContext> mockHubCallerContext = new();

            mockClients.Setup(clients => clients.Caller)
                .Returns(mockClientProxy.Object);
            mockHubCallerContext.Setup(hubCaller => hubCaller.ConnectionId)
                .Returns("77f41f68470f43a3929a1716a3a40236");

            Player[] players       = new Player[] { new(), new() };
            Room[] rooms           = new Room[] { new(), new() };
            players[0].ConnectionId = "77f41f68470f43a3929a1716a3a40236";
            players[0].Name         = "Kat";
            players[0].Room         = rooms[0];
            rooms[0].Code           = GenerateRoomCode(1411616608);
            rooms[0].Players        = new List<Player>() { players[0] };
            players[1].ConnectionId = "296e89a9aaa2473791c3f2f49a65f00a";
            players[1].Name         = "Keiran";
            players[1].Room         = rooms[1];
            rooms[1].Code           = GenerateRoomCode(647808596);
            rooms[1].Players        = new List<Player>() { players[1] };

            await _dbContext.AddRangeAsync(players);
            await _dbContext.AddRangeAsync(rooms);
            await _dbContext.SaveChangesAsync();

            RoomsHub hub = new(_dbContext, _dbContext)
            {
                Clients = mockClients.Object,
                Context = mockHubCallerContext.Object
            };

            // Act
            await hub.RecieveLeaveRoomRequest();

            // Assert
            Assert.Multiple(() =>
            {
                Room? room      = _dbContext.Rooms.Find(rooms[0].Code);
                Player? player  = _dbContext.Players.Find("77f41f68470f43a3929a1716a3a40236");

                Assert.That(room, Is.Null);
                Assert.That(player, Is.Null);
                Assert.That(_dbContext.Rooms.Count(), Is.EqualTo(1));
                Assert.That(_dbContext.Players.Count(), Is.EqualTo(1));
            });
        }

        /// <summary>
        /// Unit test for <see cref="RoomsHub.RecieveLeaveRoomRequest"/>
        /// when the <see cref="Player"/> has already joined a <see cref="Room"/>
        /// </summary>
        /// <returns>A <see cref="Task"/> representing this action</returns>
        [Test]
        public async Task RecieveLeaveRoomRequest_NoRoomJoined()
        {
            // Arrange
            Mock<IHubCallerClients<IRoomsHubClient>> mockClients    = new();
            Mock<IRoomsHubClient> mockClientProxy                   = new();
            Mock<HubCallerContext> mockHubCallerContext             = new();

            mockClients.Setup(clients => clients.Caller)
                .Returns(mockClientProxy.Object);
            mockHubCallerContext.Setup(hubCaller => hubCaller.ConnectionId)
                .Returns("4747dfecb6f84890b94a11e6f61bbcf2");

            Player[] players        = new Player[] { new(), new() };
            Room[] rooms            = new Room[] { new(), new() };
            players[0].ConnectionId = "a0ca19565f30406fa4548a8167025812";
            players[0].Name         = "Kevin";
            players[0].Room         = rooms[0];
            rooms[0].Code           = GenerateRoomCode(1495914102);
            rooms[0].Players        = new List<Player> { players[0] };
            players[1].ConnectionId = "50d2643f50024ec1ad7de23944f8b767";
            players[1].Name         = "Mary";
            players[1].Room         = rooms[1];
            rooms[1].Code           = GenerateRoomCode(1613217474);
            rooms[1].Players        = new List<Player> { players[1] };

            await _dbContext.AddRangeAsync(players);
            await _dbContext.AddRangeAsync(rooms);
            await _dbContext.SaveChangesAsync();

            RoomsHub hub = new(_dbContext, _dbContext)
            {
                Clients = mockClients.Object,
                Context = mockHubCallerContext.Object
            };

            // Act & Assert
            Assert.That(async () => await hub.RecieveLeaveRoomRequest(), 
                Throws.TypeOf<NoRoomJoinedException>());
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

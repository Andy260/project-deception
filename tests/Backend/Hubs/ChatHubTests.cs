using ProjectDeception.Backend.Hubs;
using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using DbContext = ProjectDeception.Backend.Data.DbContext;
using Microsoft.AspNetCore.SignalR;
using Moq;
using ProjectDeception.Hubs;
using ProjectDeception.Backend.Models;
using AdvancedREI;
using ProjectDeception.Exceptions;

namespace ProjectDeception.Tests.Backend.Hubs
{
    /// <summary>
    /// Test suite for <see cref="ChatHub"/>
    /// </summary>
    [TestFixture]
    public partial class ChatHubTests
    {
        // In-memory database for testing database operations
        private DbContext _dbContext;

        #region Set-up and Tear-Down

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
        /// Unit test for <see cref="ChatHub.RecieveSendRoomMessage(string)"/>
        /// </summary>
        /// <returns>A <see cref="Task"/> representing this action</returns>
        [Test]
        public async Task RecieveSendRoomMessageRequest()
        {
            // Arrange
            Room room               = new();
            Player[] players        = new Player[] { new(), new() };
            room.Code               = GenerateRoomCode(2101636600);
            room.Players            = new List<Player>(players);
            players[0].ConnectionId = "b94ca37ffe3d4c32ad8ffa134bb3c04b";
            players[0].Name         = "Neil";
            players[0].Room         = room;
            players[1].ConnectionId = "098a016ec7aa40b78af0b3b2cbb6d73c";
            players[1].Name         = "Sophie";
            players[1].Room         = room;

            await _dbContext.AddRangeAsync(players);
            await _dbContext.AddAsync(room);
            await _dbContext.SaveChangesAsync();

            Mock<IHubCallerClients<IChatHubClient>> mockClients     = new();
            Mock<IChatHubClient> mockClientProxy                    = new();
            Mock<HubCallerContext> mockHubCallerContext             = new();
            Mock<IChatHubClient> mockClientCaller                   = new();

            mockClients.Setup(clients => clients.Caller)
                .Returns(mockClientProxy.Object);

            List<string> connectionIDs = GetConnectionIDs(_dbContext.Rooms.Find(room.Code));
            mockClients.Setup(clients => clients.Clients(connectionIDs))
                .Returns(mockClientCaller.Object);

            mockHubCallerContext.Setup(hubCaller => hubCaller.ConnectionId)
                .Returns("098a016ec7aa40b78af0b3b2cbb6d73c");

            mockClientCaller.Setup(caller => caller.ReceiveRoomMessage("Hello", "Sophie"))
                .Returns(Task.CompletedTask);

            ChatHub hub = new(_dbContext, _dbContext)
            {
                Clients = mockClients.Object,
                Context = mockHubCallerContext.Object
            };

            // Act
            await hub.RecieveSendRoomMessageRequest("Hello");

            // Assert
            mockClients.Verify(clients => clients.Clients(connectionIDs), Times.Once);
            mockClientCaller.Verify(caller => caller.ReceiveRoomMessage("Hello", "Sophie"), Times.Once);
        }

        /// <summary>
        /// Unit test for <see cref="ChatHub.RecieveSendRoomMessage(string)"/>
        /// with a populated database
        /// </summary>
        /// <returns>A <see cref="Task"/> representing this action</returns>
        [Test]
        public async Task RecieveSendRoomMessageRequest_PopulatedDatabase()
        {
            // Arrange
            Room[] rooms            = new Room[] { new(), new(), new() };
            Player[] players        = new Player[] { new(), new(), new(), new(), new(), new() };
            rooms[0].Code           = GenerateRoomCode(110050466);
            rooms[0].Players        = new List<Player> { players[0], players[1], players[2] };
            players[0].ConnectionId = "8add7ddf31834917971cbc71be1a4af5";
            players[0].Name         = "Fred";
            players[0].Room         = rooms[0];
            players[1].ConnectionId = "549259d9babd4cc0b8590ad44ef3d4cd";
            players[1].Name         = "Karen";
            players[1].Room         = rooms[0];
            players[2].ConnectionId = "7c0f79cead5e43a6bd898466c2c663fc";
            players[2].Name         = "Mason";
            players[2].Room         = rooms[0];
            rooms[1].Code           = GenerateRoomCode(750466239);
            rooms[1].Players        = new List<Player> { players[3], players[4] };
            players[3].ConnectionId = "c6a4d7680a1949f2bce5cc73869faab7";
            players[3].Name         = "Sam";
            players[3].Room         = rooms[1];
            players[4].ConnectionId = "41eafffb5381462e8a89e04865c59925";
            players[4].Name         = "Ben";
            players[4].Room         = rooms[1];
            rooms[2].Code           = GenerateRoomCode(289330634);
            rooms[2].Players        = new List<Player> { players[5] };
            players[5].ConnectionId = "9724fb1ab9454cd9a5ffc18aad606134";
            players[5].Name         = "Neil";
            players[5].Room         = rooms[2];

            await _dbContext.AddRangeAsync(players);
            await _dbContext.AddRangeAsync(rooms);
            await _dbContext.SaveChangesAsync();

            Mock<IHubCallerClients<IChatHubClient>> mockClients = new();
            Mock<IChatHubClient> mockClientProxy = new();
            Mock<HubCallerContext> mockHubCallerContext = new();
            Mock<IChatHubClient> mockClientCaller = new();

            mockClients.Setup(clients => clients.Caller)
                .Returns(mockClientProxy.Object);

            List<string> connectionIDs = GetConnectionIDs(_dbContext.Rooms.Find(rooms[0].Code));
            mockClients.Setup(clients => clients.Clients(connectionIDs))
                .Returns(mockClientCaller.Object);

            mockHubCallerContext.Setup(hubCaller => hubCaller.ConnectionId)
                .Returns("7c0f79cead5e43a6bd898466c2c663fc");

            mockClientCaller.Setup(caller => caller.ReceiveRoomMessage("Hello", "Sophie"))
                .Returns(Task.CompletedTask);

            ChatHub hub = new(_dbContext, _dbContext)
            {
                Clients = mockClients.Object,
                Context = mockHubCallerContext.Object
            };

            // Act
            await hub.RecieveSendRoomMessageRequest("What?");

            // Assert
            mockClients.Verify(clients => clients.Clients(connectionIDs), Times.Once);
            mockClientCaller.Verify(caller => caller.ReceiveRoomMessage("What?", "Mason"), Times.Once);
        }

        /// <summary>
        /// Unit test for <see cref="ChatHub.RecieveSendRoomMessage(string)"/>
        /// from a user who isn't in a room
        /// </summary>
        /// <returns>A <see cref="Task"/> representing this action</returns>
        [Test]
        public async Task RecieveSendRoomMessageRequest_NotInRoom()
        {
            // Arrange
            Room[] rooms            = new Room[] { new(), new() };
            Player[] players        = new Player[] { new(), new() };
            rooms[0].Code           = GenerateRoomCode(716370769);
            rooms[0].Players        = new List<Player> { players[0] };
            players[0].ConnectionId = "2d237fcc9afa4cc0a57cb52ad02b5e51";
            players[0].Name         = "Karen";
            players[0].Room         = rooms[0];
            rooms[1].Code           = GenerateRoomCode(1757473418);
            rooms[1].Players        = new List<Player> { players[1] };
            players[1].ConnectionId = "a876f55d888d49da9f00be2997bc97d4";
            players[1].Name         = "Daisy";
            players[1].Room         = rooms[1];

            await _dbContext.AddRangeAsync(rooms);
            await _dbContext.AddRangeAsync(players);
            await _dbContext.SaveChangesAsync();

            Mock<IHubCallerClients<IChatHubClient>> mockClients = new();
            Mock<IChatHubClient> mockClientProxy = new();
            Mock<HubCallerContext> mockHubCallerContext = new();

            mockClients.Setup(clients => clients.Caller)
                .Returns(mockClientProxy.Object);

            mockHubCallerContext.Setup(hubCaller => hubCaller.ConnectionId)
                .Returns("04b9ee5f2d384196a5dba7ec821fa205");

            ChatHub hub = new(_dbContext, _dbContext)
            {
                Clients = mockClients.Object,
                Context = mockHubCallerContext.Object
            };

            // Act & Assert
            Assert.That(async () => await hub.RecieveSendRoomMessageRequest("Did this work?"),
                    Throws.TypeOf<NoRoomJoinedException>());
        }

        #endregion

        #region Helper Functions

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

        private static List<string> GetConnectionIDs(Room? room)
        {
            if (room == null)
            {
                throw new ArgumentNullException(nameof(room));
            }
            else if (room.Players == null)
            {
                throw new ArgumentException(null, nameof(room));
            }

            List<string> connectionIDs = new(room.Players.Count);
            foreach (Player? player in room.Players)
            {
                if (player == null || player.ConnectionId == null)
                {
                    continue;
                }

                connectionIDs.Add(player.ConnectionId);
            }

            return connectionIDs;
        }

        #endregion
    }
}

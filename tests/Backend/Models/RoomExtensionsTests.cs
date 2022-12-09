using ProjectDeception.Backend.Models;
using NUnit.Framework;

namespace ProjectDeception.Tests.Backend.Models
{
    /// <summary>
    /// Test suite for <see cref="RoomExtensions"/>
    /// </summary>
    [TestFixture]
    public class RoomExtensionsTests
    {
        #region Public Function Tests

        /// <summary>
        /// Unit test for the <see cref="RoomExtensions.GetConnectionIDs(Room)"/>
        /// method
        /// </summary>
        [Test]
        public void GetConnectionIDs()
        {
            // Arrange
            Room room = new();
            Player[] players = new Player[]
            {
                new()
                {
                    Name            = "Jane",
                    ConnectionId    = "52ac2662c876463cabce2f09f1098ea4",
                    Room            = room
                },
                new()
                {
                    Name            = "Jeff",
                    ConnectionId    = "ca4597dd864c4aa197745184c76e5050",
                    Room            = room
                },
                new()
                {
                    Name            = "Bailey",
                    ConnectionId    = "5e4a0351fbca4d3eac792af0ebe63a6b",
                    Room            = room
                }
            };
            room.Code       = "MAJV8A";
            room.Players    = new List<Player>(players);

            // Act
            IList<string> result = room.GetConnectionIDs();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Has.Count.EqualTo(players.Length));
                foreach (Player player in players)
                {
                    Assert.That(result, Contains.Item(player.ConnectionId));
                }
            });
        }

        /// <summary>
        /// Unit test for the <see cref="RoomExtensions.GetConnectionIDs(Room)"/>
        /// with an empty <see cref="Room"/>
        /// </summary>
        [Test]
        public void GetConnectionIDs_EmptyRoom()
        {
            // Arrange
            Room room = new()
            {
                Code    = "BJCG12",
                Players = new List<Player>()
            };

            // Act & Assert
            Assert.That(() => room.GetConnectionIDs(), 
                Throws.TypeOf<ArgumentException>());
        }

        /// <summary>
        /// Unit test for the <see cref="RoomExtensions.GetConnectionIDs(Room)"/>
        /// with a <see cref="Room"/> which has a null <see cref="Player"/> list
        /// </summary>
        [Test]
        public void GetConnectionIDs_NullPlayerList()
        {
            // Arrange
            Room room = new()
            {
                Code    = "JKALEA",
                Players = null
            };

            // Act & Assert
            Assert.That(() => room.GetConnectionIDs(),
                Throws.TypeOf<ArgumentException>());
        }

        /// <summary>
        /// Unit test for the <see cref="RoomExtensions.GetConnectionIDs(Room)"/>
        /// method with invalid <see cref="Player"/> objects in the <see cref="Room"/>
        /// </summary>
        [Test]
        public void GetConnectionIDs_NullConnectionID() 
        {
            // Arrange
            Room room           = new();
            Player[] players    = new Player[]
            {
                new()
                {
                    Name            = "Ron",
                    ConnectionId    = null,
                    Room            = room,
                }
            };
            room.Code       = "72N2LA";
            room.Players    = new List<Player>(players);

            // Act & Assert
            Assert.That(() => room.GetConnectionIDs(),
                Throws.TypeOf<ArgumentException>());
        }

        /// <summary>
        /// Unit test for the <see cref="RoomExtensions.GetConnectionIDs(Room)"/>
        /// method with a <see cref="Player"/> which has an empty connection ID
        /// </summary>
        [Test]
        public void GetConnectionIDs_EmptyConnectionID()
        {
            // Arrange
            Room room = new();
            Player[] players = new Player[]
            {
                new()
                {
                    Name            = "Gerald",
                    ConnectionId    = "",
                    Room            = room,
                }
            };
            room.Code       = "1TRY9A";
            room.Players    = new List<Player>(players);

            // Act & Assert
            Assert.That(() => room.GetConnectionIDs(),
                Throws.TypeOf<ArgumentException>());
        }

        /// <summary>
        /// Unit test for the <see cref="RoomExtensions.GetConnectionIDs(Room)"/>
        /// method with a <see cref="Player"/> which has an connection ID
        /// containing only white space
        /// </summary>
        [Test]
        public void GetConnectionIDs_WhiteSpaceConnectionID()
        {
            // Arrange
            Room room = new();
            Player[] players = new Player[]
            {
                new()
                {
                    Name            = "Ben",
                    ConnectionId    = " ",
                    Room            = room,
                }
            };
            room.Code       = "026ANA";
            room.Players    = new List<Player>(players);

            // Act & Assert
            Assert.That(() => room.GetConnectionIDs(),
                Throws.TypeOf<ArgumentException>());
        }

        /// <summary>
        /// Unit test for the <see cref="RoomExtensions.GetConnectionIDs(Room)"/>
        /// method with a NULL <see cref="Room"/>
        /// </summary>
        [Test]
        public void GetConnectionIDs_NullRoom()
        {
            // Arrange
            Room? room = null;

            // Act & Assert
#pragma warning disable CS8604 // Possible null reference argument.
            Assert.That(() => room.GetConnectionIDs(),
                Throws.TypeOf<ArgumentNullException>());
#pragma warning restore CS8604 // Possible null reference argument.
        }

        #endregion
    }
}

using ProjectDeception.Rooms;
using NUnit.Framework;

namespace ProjectDeception.Tests
{
    /// <summary>
    /// Test suite for <see cref="RoomUtilities"/>
    /// </summary>
    [TestFixture]
    public class RoomUtilitiesTests
    {
        #region Public Function Tests

        [Test]
        [TestCase("9ANM01")]
        [TestCase("RM5N17")]
        [TestCase("QL7N9A")]
        public void IsValidRoomCode(string roomCode)
        {
            // Act & Assert
            Assert.That(RoomUtilities.IsValidRoomCode(roomCode), Is.True);
        }

        [Test]
        public void IsValidRoomCode_Whitespace()
        {
            // Act & Assert
            Assert.That(RoomUtilities.IsValidRoomCode(" "), Is.False);
        }

        [Test]
        [TestCase("dsfsv01")]
        [TestCase("JKAL0915")]
        [TestCase("u6qe57")]
        [TestCase("100000")]
        public void IsValidRoomCode_InvalidCode(string roomCode)
        {
            // Act & Assert
            Assert.That(RoomUtilities.IsValidRoomCode(roomCode), Is.False);
        }

        #endregion
    }
}

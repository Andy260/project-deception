using ProjectDeception.Backend.Models;
using NUnit.Framework;
using System.Text.RegularExpressions;

namespace ProjectDeception.Tests.Backend.Models
{
    /// <summary>
    /// Test suite for <see cref="RoomUtilities"/>
    /// </summary>
    [TestFixture]
    public partial class RoomUtilitiesTests
    {
        // Regular expression for
        // testing generated room codes
        [GeneratedRegex("[A-Z, 0-9]{6}")]
        private static partial Regex RoomCodeRegex();

        #region Public Function Tests

        /// <summary>
        /// Unit test for <see cref="RoomUtilities.GenerateRoomCode(int)"/>
        /// </summary>
        [Test]
        [TestCase(60466176)]
        [TestCase((int.MaxValue - 60466176) / 2)]
        [TestCase(int.MaxValue)]
        public void GenerateRoomCode(int value)
        {
            // Act
            string result = RoomUtilities.GenerateRoomCode(value);

            // Assert
            Assert.That(IsValidRoomCode(result), Is.True);
        }

        /// <summary>
        /// Unit test for <see cref="RoomUtilities.GenerateRoomCode(int)"/>
        /// with an invalid value
        /// </summary>
        [Test]
        [TestCase(60466176 - 1)]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-60466176)]
        public void GenerateRoomName_InvalidValue(int value)
        {
            // Act & Assert
            Assert.That(() => RoomUtilities.GenerateRoomCode(value), 
                Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        /// <summary>
        /// Unit test for <see cref="RoomUtilities.GenerateRandomRoomCode"/>
        /// </summary>
        [Test]
        public void GenerateRandomRoomCode()
        {
            // Act
            string result = RoomUtilities.GenerateRandomRoomCode();

            // Assert
            Assert.That(IsValidRoomCode(result), Is.True);
        }

        #endregion

        #region Helper Functions

        private static bool IsValidRoomCode(string code)
        {
            return RoomCodeRegex().IsMatch(code);
        }

        #endregion
    }
}

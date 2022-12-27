using NUnit.Framework;
using ProjectDeception.Frontend.Models.Validation;

namespace ProjectDeception.Tests.Frontend.Models.Validation
{
    /// <summary>
    /// Test suite for <see cref="RoomCodeAttribute"/>
    /// </summary>
    [TestFixture]
    public class RoomCodeAttributeTests
    {
        #region Nested Types

        private class ValidModel
        {
            [RoomCode]
            public string? RoomCode { get; set; }
        }

        private class InvalidModel
        {
            [RoomCode]
            public int RoomCode { get; set; }
        }

        #endregion

        #region Unit Tests

        /// <summary>
        /// Unit test for the intended usage 
        /// of <see cref="RoomCodeAttribute"/>
        /// </summary>
        /// <param name="roomCode">Room code to test with</param>
        [Test]
        [TestCase("HNA890")]
        [TestCase("1AN5PU")]
        [TestCase("RTAB01")]
        public void ExpectedUsage(string roomCode)
        {
            // Arrange
            ValidModel model = new()
            {
                RoomCode = roomCode
            };
            RoomCodeAttribute attribute = new();
            
            // Act
            bool isValid = attribute.IsValid(model.RoomCode);

            // Assert
            Assert.That(isValid, Is.True);
        }

        /// <summary>
        /// Unit test for <see cref="RoomCodeAttribute"/> 
        /// with an invalid room code
        /// </summary>
        /// <param name="roomCode"></param>
        [Test]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("H98d8a")]
        [TestCase("APO")]
        [TestCase("100000")]
        public void InvalidRoomCode(string roomCode)
        {
            // Arrange
            ValidModel model = new()
            {
                RoomCode = roomCode
            };
            RoomCodeAttribute attribute = new();

            // Act
            bool isValid = attribute.IsValid(model.RoomCode);

            // Assert
            Assert.That(isValid, Is.False);
        }

        /// <summary>
        /// Unit test for <see cref="RoomCodeAttribute"/>
        /// with a null value
        /// </summary>
        [Test]
        public void NullCode()
        {
            // Arrange
            ValidModel model = new()
            {
                RoomCode = ""
            };
            RoomCodeAttribute attribute = new();

            // Act
            bool isValid = attribute.IsValid(model.RoomCode);

            // Assert
            Assert.That(isValid, Is.False);
        }

        /// <summary>
        /// Unit test for <see cref="RoomCodeAttribute"/>
        /// on an invalid property
        /// </summary>
        [Test]
        public void InvalidUsage()
        {
            // Arrange
            InvalidModel model = new()
            {
                RoomCode = 10
            };
            RoomCodeAttribute attribute = new();

            // Act & Assert
            Assert.That(() => attribute.IsValid(model.RoomCode), 
                Throws.TypeOf<InvalidOperationException>());
        }

        #endregion
    }
}

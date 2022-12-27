using ProjectDeception.Frontend.Models;
using NUnit.Framework;

namespace ProjectDeception.Tests.Frontend.Models
{
    /// <summary>
    /// Test suite for <see cref="JoinRoomModel"/>
    /// </summary>
    [TestFixture]
    public class JoinRoomModelTests
    {
        #region Public Function Tests

        [Test]
        public void Reset()
        {
            // Arrange
            JoinRoomModel model = new JoinRoomModel
            {
                Name        = "Jane",
                RoomCode    = "HBK90A"
            };

            // Act
            model.Reset();

            // Assert
            Assert.Multiple(() =>
            {
                JoinRoomModel newModel = new();

                Assert.That(model.Name, Is.EqualTo(newModel.Name));
                Assert.That(model.RoomCode, Is.EqualTo(newModel.RoomCode));
            });
        }

        #endregion
    }
}

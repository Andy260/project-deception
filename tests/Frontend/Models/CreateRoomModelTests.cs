using ProjectDeception.Frontend.Models;
using NUnit.Framework;

namespace ProjectDeception.Tests.Frontend.Models
{
    /// <summary>
    /// Test suite for <see cref="CreateRoomModel"/>
    /// </summary>
    [TestFixture]
    public class CreateRoomModelTests
    {
        #region Public Function Tests

        [Test]
        public void Reset()
        {
            // Arrange
            CreateRoomModel model = new CreateRoomModel
            {
                Name = "Zoe"
            };

            // Act
            model.Reset();

            // Assert
            Assert.Multiple(() =>
            {
                CreateRoomModel newModel = new();

                Assert.That(model.Name, Is.EqualTo(newModel.Name));
            });
        }

        #endregion
    }
}

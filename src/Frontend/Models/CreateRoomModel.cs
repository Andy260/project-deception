using System.ComponentModel.DataAnnotations;

namespace ProjectDeception.Frontend.Models
{
    /// <summary>
    /// Form model for creating a room
    /// </summary>
    internal class CreateRoomModel : IModel
    {
        #region Properties

        /// <summary>
        /// Intended name for the user
        /// </summary>
        [Required]
        [StringLength(32, ErrorMessage = "Name cannot be more than 32 characters")]
        public string Name { get; set; } = "";

        #endregion

        #region Public Functions

        /// <inheritdoc/>
        public void Reset()
        {
            Name = "";
        }

        #endregion
    }
}

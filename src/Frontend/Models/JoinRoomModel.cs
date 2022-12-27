using System.ComponentModel.DataAnnotations;
using ProjectDeception.Frontend.Models.Validation;

namespace ProjectDeception.Frontend.Models
{
    /// <summary>
    /// Form model for joining a room
    /// </summary>
    internal class JoinRoomModel : IModel
    {
        #region Properties

        /// <summary>
        /// Intended name for the user
        /// </summary>
        [Required]
        [StringLength(32, ErrorMessage = "Name cannot be more than 32 characters")]
        public string Name { get; set; } = "";

        /// <summary>
        /// Code of the room to join
        /// </summary>
        [Required]
        [RoomCode]
        public string RoomCode { get; set; } = "";

        #endregion

        #region Public Functions

        /// <inheritdoc/>
        public void Reset()
        {
            Name        = "";
            RoomCode    = "";
        }

        #endregion
    }
}

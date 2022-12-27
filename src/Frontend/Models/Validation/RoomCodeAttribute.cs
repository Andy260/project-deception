using System.ComponentModel.DataAnnotations;
using ProjectDeception.Rooms;

namespace ProjectDeception.Frontend.Models.Validation
{
    /// <summary>
    /// Specifies that a string must be a valid room code
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class RoomCodeAttribute : ValidationAttribute
    {
        #region Properties

        /// <inheritdoc/>
        public override bool RequiresValidationContext => false;

        #endregion

        #region Public Functions

        /// <inheritdoc/>
        public override bool IsValid(object? value)
        {
            // Ensure given value is a string
            if (value is not string)
            {
                throw new InvalidOperationException("Must be used on a string");
            }

            return RoomUtilities.IsValidRoomCode((string)value);
        }

        #endregion
    }
}

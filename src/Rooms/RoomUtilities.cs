using System.Text.RegularExpressions;

namespace ProjectDeception.Rooms
{
    /// <summary>
    /// Helper class for various methods
    /// assisting with Rooms
    /// </summary>
    public static partial class RoomUtilities
    {
        // Regular expression for
        // testing generated room codes
        [GeneratedRegex("[A-Z, 0-9]{6}")]
        private static partial Regex RoomCodeRegex();

        /// <summary>
        /// Is the given room code valid?
        /// </summary>
        /// <param name="roomCode">Room code to check</param>
        /// <returns>True if the given room code is valid</returns>
        public static bool IsValidRoomCode(string roomCode)
        {
            if (string.IsNullOrWhiteSpace(roomCode) || roomCode.Length > 6)
            {
                return false;
            }

            return RoomCodeRegex().IsMatch(roomCode);
        }
    }
}

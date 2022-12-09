using AdvancedREI;

namespace ProjectDeception.Backend.Models
{
    /// <summary>
    /// Helper class for the <see cref="Room"/> model
    /// </summary>
    public static class RoomUtilities
    {
        #region Public Functions

        /// <summary>
        /// Generates a <see cref="Room"/> code from a given value
        /// </summary>
        /// <param name="value">Generated <see cref="Room"/> code</param>
        /// <returns>Generated <see cref="Room"/> code</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="value"/> is not within the range
        /// of 60466176 - <see cref="int.MaxValue"/>
        /// </exception>
        public static string GenerateRoomCode(int value)
        {
            // Ensure given value is within limits
            // (Room codes are actually a base 36 representation,
            // or [A-Z, 0-9]{6} as a regular expression, with the
            // minimum value being 100000 (or 60466176 in base10)
            // so it's guaranteed to return a code with six characters
            if (value < 60466176 || value > int.MaxValue || value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            return Base36.NumberToBase36(value);
        }

        /// <summary>
        /// Generates a <see cref="Room"/> code from a random value
        /// </summary>
        /// <returns>Generated <see cref="Room"/> code</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="value"/> is not within the range
        /// of 60466176 - <see cref="int.MaxValue"/>
        public static string GenerateRandomRoomCode()
        {
            return GenerateRoomCode(Random.Shared.Next(60466176, int.MaxValue));
        }

        #endregion
    }
}

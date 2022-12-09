namespace ProjectDeception.Backend.Models
{
    /// <summary>
    /// Extension methods for <see cref="Room"/> 
    /// </summary>
    public static class RoomExtensions
    {
        #region Public Functions

        /// <summary>
        /// Gets all the connection IDs of each <see cref="Player"/>
        /// in this <see cref="Room"/>
        /// </summary>
        /// <param name="room"><see cref="Room"/> to get connection IDs from</param>
        /// <returns>Connection IDs of each <see cref="Player"/> in the <see cref="Room"/></returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="room"/> is null
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when the player list of <paramref name="room"/> is invalid
        /// (List is null, contains no players, or a player contains a null connection ID, 
        /// or connection ID is white space)
        /// </exception>
        public static IList<string> GetConnectionIDs(this Room room)
        {
            // Ensure room isn't null
            if (room == null)
            {
                throw new ArgumentNullException(nameof(room));
            }
            // Ensure room player list isn't null
            else if (room.Players == null)
            {
                throw new ArgumentException("Room player list is null",
                    nameof(room));
            }
            // Ensure room player list isn't empty
            else if (room.Players.Count < 1)
            {
                throw new ArgumentException("Room player list contains no players",
                    nameof(room));
            }

            List<string> connectionIDs = new List<string>(room.Players.Count);
            foreach (Player player in room.Players)
            {
                // Ensure player connection ID is valid
                if (string.IsNullOrWhiteSpace(player.ConnectionId))
                {
                    throw new ArgumentException("Room player list contains " +
                        "player with invalid connection ID", nameof(room));
                }

                connectionIDs.Add(player.ConnectionId);
            }

            return connectionIDs;
        }

        #endregion
    }
}

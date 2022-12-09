namespace ProjectDeception.Hubs
{
    /// <summary>
    /// Interface for clients interacting with the Rooms Hub
    /// </summary>
    public interface IRoomsHubClient
    {
        /// <summary>
        /// Requests to create a room
        /// </summary>
        /// <param name="playerName">Preferred name of the player creating this room</param>
        /// <returns>Room code of created room, null if no room was created</returns>
        Task<string> RequestCreateRoom(string playerName);

        /// <summary>
        /// Requests to join a room
        /// </summary>
        /// <param name="roomCode">Code of the room to join</param>
        /// <returns>A <see cref="Task"/> representing this action</returns>
        Task RequestJoinRoom(string roomCode, string playerName);

        /// <summary>
        /// Requests to leave the currently joined room
        /// (has no effect if not currently in a room)
        /// </summary>
        /// <returns>A <see cref="Task"/> representing this action</returns>
        Task RequestLeaveRoom();
    }
}

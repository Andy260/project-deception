namespace ProjectDeception
{
    /// <summary>
    /// Interface for clients interacting with the Rooms Hub
    /// </summary>
    public interface IRoomsHubClient
    {
        /// <summary>
        /// Requests to create a room
        /// </summary>
        /// <returns>Room code of created room, null if no room was created</returns>
        Task<string> RequestCreateRoom();
    }
}

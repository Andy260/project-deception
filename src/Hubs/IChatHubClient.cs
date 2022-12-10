namespace ProjectDeception.Hubs
{
    /// <summary>
    /// Interface for clients interacting with the Chat hub
    /// </summary>
    public interface IChatHubClient
    {
        /// <summary>
        /// Sends a message to all players in the connected room
        /// </summary>
        /// <param name="message">Message to send</param>
        /// <returns>A <see cref="Task"/> representing this action</returns>
        Task SendRoomMessage(string message);

        /// <summary>
        /// Receives a message from another player, 
        /// send to all players in the room
        /// </summary>
        /// <param name="message">Message received</param>
        /// <param name="playerName">Player which sent the message</param>
        /// <returns>A <see cref="Task"/> representing this action</returns>
        Task ReceiveRoomMessage(string message, string playerName);
    }
}

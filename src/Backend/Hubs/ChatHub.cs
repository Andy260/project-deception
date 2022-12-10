using Microsoft.AspNetCore.SignalR;
using ProjectDeception.Backend.Data;
using ProjectDeception.Backend.Models;
using ProjectDeception.Exceptions;
using ProjectDeception.Hubs;

namespace ProjectDeception.Backend.Hubs
{
    /// <summary>
    /// SignalR <see cref="Hub"/> for players to chat within a <see cref="Room"/>
    /// </summary>
    public class ChatHub : Hub<IChatHubClient>
    {
        // Player data
        private readonly IPlayerData _playersData;
        // Room data
        private readonly IRoomsData _roomsData;

        #region Constructors

        /// <summary>
        /// Constructs a new <see cref="ChatHub"/>
        /// </summary>
        /// <param name="playerData"><see cref="Player"/> data</param>
        /// <param name="roomsData"><see cref="Room"/> data</param>
        public ChatHub(IPlayerData playerData, IRoomsData roomsData)
        {
            _playersData    = playerData;
            _roomsData      = roomsData;
        }

        #endregion

        #region Client Requests

        /// <summary>
        /// Responds to a request from a client to send a 
        /// message to all clients within a room a message
        /// </summary>
        /// <param name="message">Message to send</param>
        /// <returns>A <see cref="Task"/> representing this action</returns>
        /// <exception cref="NoRoomJoinedException">Thrown when caller isn't in a <see cref="Room"/></exception>
        /// <exception cref="Exception">Thrown when caller hasn't got a valid name</exception>
        [HubMethodName(nameof(IChatHubClient.SendRoomMessage))]
        public async Task RecieveSendRoomMessageRequest(string message)
        {
            // Find caller in database
            Player? caller = await _playersData.Players.FindAsync(Context.ConnectionId);
            if (caller == null || caller.Room == null)
            {
                // Caller not currently in a room
                throw new NoRoomJoinedException();
            }
            else if (string.IsNullOrWhiteSpace(caller.Name))
            {
                throw new Exception("Caller has invalid name");
            }

            // Alert all users in the caller's room
            // of the message
            IChatHubClient playersInRoom = Clients.Clients(caller.Room.GetConnectionIDs());
            await playersInRoom.ReceiveRoomMessage(message, caller.Name);
        }

        #endregion

        #region Helper Functions

        private static List<string> GetConnectionIDsFromRoom(Room? room)
        {
            // Ensure arguments aren't null
            if (room == null)
            {
                throw new ArgumentNullException(nameof(room));
            }
            else if (room.Players == null)
            {
                throw new ArgumentException("Player list is NULL", nameof(room));
            }

            // Get connection IDs of players in the room
            List<string> connectionIDs = new(room.Players.Count);
            foreach (Player? player in room.Players)
            {
                if (player == null || player.ConnectionId == null)
                {
                    continue;
                }

                connectionIDs.Add(player.ConnectionId);
            }

            return connectionIDs;
        }

        #endregion
    }
}

using AdvancedREI;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ProjectDeception.Backend.Data;
using ProjectDeception.Backend.Models;
using ProjectDeception.Exceptions;
using ProjectDeception.Hubs;

namespace ProjectDeception.Backend.Hubs
{
    /// <summary>
    /// SignalR Hub for clients managing rooms
    /// </summary>
    public class RoomsHub : Hub<IRoomsHubClient>
    {
        // Player data
        private readonly IPlayerData _playerData;
        // Rooms data
        private readonly IRoomsData _roomsData;

        #region Constructors

        /// <summary>
        /// Constructs a new <see cref="RoomsHub"/>
        /// </summary>
        /// <param name="playerData"><see cref="Player"/> data</param>
        /// <param name="roomsData"><see cref="Room"/> data</param>
        public RoomsHub(IPlayerData playerData, IRoomsData roomsData)
        {
            _playerData = playerData;
            _roomsData  = roomsData;
        }

        #endregion

        #region Client Requests

        /// <summary>
        /// Responds to a request from a client to create a <see cref="Room"/> 
        /// </summary>
        /// <returns>Code for created <see cref="Room"/></returns>
        /// <param name="playerName"></param>
        /// <exception cref="NotImplementedException"></exception>
        [HubMethodName(nameof(IRoomsHubClient.RequestCreateRoom))]
        public async Task<string> RecieveCreateRoomRequest(string playerName)
        {
            // Generate room code
            string roomCode = GenerateRoomCode();

            // Create new player
            EntityEntry<Player> newPlayer = await _playerData.Players.AddAsync(new Player 
            { 
                ConnectionId    = Context.ConnectionId, 
                Name            = playerName 
            });
            // Create new room
            EntityEntry<Room> newRoom = await _roomsData.Rooms.AddAsync(new Room 
            { 
                Code = roomCode
            });

            // Connect newly created room and player together
            newPlayer.Entity.Room   = newRoom.Entity;
            newRoom.Entity.Players  = new List<Player>() { newPlayer.Entity };

            // Update database
            await _playerData.SaveChangesAsync();
            await _roomsData.SaveChangesAsync();

            return roomCode;
        }

        /// <summary>
        /// Responds to a client's request to join a <see cref="Room"/>
        /// </summary>
        /// <param name="roomCode">Code of the <see cref="Room"/> for the <see cref="Player"/> to join</param>
        /// <param name="playerName">Name of the <see cref="Player"/> joining the <see cref="Room"/></param>
        /// <returns><see cref="Task"/> representing this action</returns>
        /// <exception cref="AlreadyInRoomException">
        /// Thrown when the caller is already in a <see cref="Room"/>
        /// </exception>
        /// <exception cref="InvalidRoomCodeException">
        /// Thrown when the caller provides an invalid <see cref="Room"/> code
        /// </exception>
        [HubMethodName(nameof(IRoomsHubClient.RequestJoinRoom))]
        public async Task RecieveJoinRoomRequest(string roomCode, string playerName)
        {
            // Check if user is already in another room
            Player? caller = await _playerData.Players.FindAsync(Context.ConnectionId);
            if (caller != null && caller.Room != null)
            {
                // User is already in a room
                throw new AlreadyInRoomException();
            }

            // Find the requested room
            Room? room = await _roomsData.Rooms.FindAsync(roomCode);
            if (room == null)
            {
                // Room doesn't exist
                throw new InvalidRoomCodeException();
            }

            if (caller != null)
            {
                // Player already in the database,
                // update room and name
                caller.Room = room;
                caller.Name = playerName;
            }
            else
            {
                // Create new player in database
                caller = new Player
                {
                    ConnectionId    = Context.ConnectionId,
                    Name            = playerName,
                    Room            = room
                };
                await _playerData.Players.AddAsync(caller);
            }

            // TODO: Notify player of current room state
            // (players in the room, game state, etc)

            // Save changes to database
            await _playerData.SaveChangesAsync();
        }

        /// <summary>
        /// Responds to a client's request to leave the 
        /// <see cref="Room"/> they have joined
        /// </summary>
        /// <returns><see cref="Task"/> representing this action</returns>
        [HubMethodName(nameof(IRoomsHubClient.RequestLeaveRoom))]
        public async Task RecieveLeaveRoomRequest()
        {
            Player? caller = await _playerData.Players.FindAsync(Context.ConnectionId);
            if (caller == null || caller.Room == null)
            {
                // Player hasn't joined a room
                throw new NoRoomJoinedException();
            }

            Room? room = caller.Room;
            if (room != null && room.Players != null)
            {
                // Remove player from room
                room.Players.Remove(caller);

                // Check if there's any
                // players left in the room
                if (room.Players.Count == 0)
                {
                    // Destroy room
                    _roomsData.Rooms.Remove(room);
                }
            }
            else
            {
                // Invalid data found in database
                // TODO: Provide better error messages for invalid database data with rooms and/or players
                throw new Exception("Invalid Room data");
            }

            // Destroy player data
            _playerData.Players.Remove(caller);

            // Update database
            await Task.WhenAll(_playerData.SaveChangesAsync(), _roomsData.SaveChangesAsync());

            // TODO: Notify other players in the room of player leaving
        }

        #endregion

        #region Helper Functions

        private string GenerateRoomCode()
        {
            // Room codes are actually a base 36 representation,
            // or [A-Z, 0-9]{6} as a regular expression, with the
            // minimum value being 100000 (or 60466176 in base10)
            // so it's guaranteed to return a code with six characters
            int seed = Random.Shared.Next(60466176, int.MaxValue);

            // Generate a unique room code
            string? roomCode = null;
            while (roomCode == null)
            {
                string generatedCode = Base36.NumberToBase36(seed);
                // Ensure this code isn't already in use
                if (_roomsData.Rooms.Any(room => room.Code == generatedCode))
                {
                    // Code already in use
                    continue;
                }
                else
                {
                    // Code not in use
                    roomCode = generatedCode;
                }
            }

            return roomCode;
        }

        #endregion
    }
}

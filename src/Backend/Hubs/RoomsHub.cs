using AdvancedREI;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ProjectDeception.Backend.Data;
using ProjectDeception.Backend.Models;

namespace ProjectDeception.Backend.Hubs
{
    /// <summary>
    /// SignalR Hub for clients managing rooms
    /// </summary>
    public class RoomsHub : Hub<IRoomsHubClient>
    {
        // Rooms data
        private readonly IRoomsData _roomsData;

        #region Constructors

        /// <summary>
        /// Constructs a new <see cref="RoomsHub"/>
        /// </summary>
        /// <param name="roomsData"><see cref="Room"/>s data</param>
        public RoomsHub(IRoomsData roomsData)
        {
            _roomsData = roomsData;
        }

        #endregion

        #region Client Requests

        /// <summary>
        /// Responds to a request from a client to create a room 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [HubMethodName(nameof(IRoomsHubClient.RequestCreateRoom))]
        public async Task<string> RecieveCreateRoomRequest()
        {
            // Generate room code
            string? roomCode = null;
            while (roomCode == null)
            {
                string generatedCode = GenerateRoomCode(Random.Shared.Next(60466176, int.MaxValue));
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

            // Create room
            EntityEntry<Room> roomEntity = await _roomsData.Rooms.AddAsync(new Room { Code = roomCode });
            await _roomsData.SaveChangesAsync();

            return roomCode;
        }

        #endregion

        #region Helper Functions

        private static string GenerateRoomCode(int seed)
        {
            // Ensure seed is within limits
            // (Room codes are actually a base 36 representation,
            // or [A-Z, 0-9]{6} as a regular expression, with the
            // minimum value being 100000 (or 60466176 in base10)
            // so it's guaranteed to return a code with six characters
            if (seed < 60466176 || seed > int.MaxValue || seed < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(seed));
            }

            return Base36.NumberToBase36(seed);
        }

        #endregion
    }
}

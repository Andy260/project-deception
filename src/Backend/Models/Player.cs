using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.SignalR;

namespace ProjectDeception.Backend.Models
{
    /// <summary>
    /// Representation of a <see cref="Player"/>
    /// </summary>
    public class Player
    {
        /// <summary>
        /// Connection of this <see cref="Player"/> 
        /// represented by <see cref="HubCallerContext.ConnectionId"/>
        /// </summary>
        [Key]
        [Required]
        public string? ConnectionId { get; set; }

        /// <summary>
        /// Display name of this <see cref="Player"/>
        /// </summary>
        [Required]
        [StringLength(32, MinimumLength = 2)]
        public string? Name { get; set; }

        /// <summary>
        /// <see cref="Models.Room"/> this 
        /// <see cref="Player"/> is playing within
        /// </summary>
        [Required]
        public Room? Room { get; set; }
    }
}

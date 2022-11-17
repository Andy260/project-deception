using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ProjectDeception.Backend.Models
{
    /// <summary>
    /// Model describing a room
    /// </summary>
    public class Room
    {
        /// <summary>
        /// Access code to this <see cref="Room"/>
        /// </summary>
        [Key]
        [Required]
        [MaxLength(6)]
        [Unicode(false)]
        public string? Code { get; set; }
    }
}

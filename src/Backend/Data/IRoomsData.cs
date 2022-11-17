using Microsoft.EntityFrameworkCore;
using ProjectDeception.Backend.Models;

namespace ProjectDeception.Backend.Data
{
    /// <summary>
    /// Interface for interacting with data 
    /// associated with <see cref="Room"/>s
    /// </summary>
    public interface IRoomsData
    {
        /// <summary>
        /// Created <see cref="Room"/>s
        /// </summary>
        DbSet<Room> Rooms { get; }

        int SaveChanges();
        int SaveChanges(bool acceptAllChangesOnSuccess);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, 
            CancellationToken cancellationToken = default);
    }
}

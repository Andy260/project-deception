using Microsoft.EntityFrameworkCore;
using ProjectDeception.Backend.Models;

namespace ProjectDeception.Backend.Data
{
    /// <summary>
    /// Interface for interacting with data 
    /// associated with active players
    /// </summary>
    public interface IPlayerData
    {
        /// <summary>
        /// Active players
        /// </summary>
        DbSet<Player> Players { get; }

        int SaveChanges();
        int SaveChanges(bool acceptAllChangesOnSuccess);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default);
    }
}

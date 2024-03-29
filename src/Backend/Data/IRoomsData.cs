﻿using Microsoft.EntityFrameworkCore;
using ProjectDeception.Backend.Models;

namespace ProjectDeception.Backend.Data
{
    /// <summary>
    /// Interface for interacting with data 
    /// associated with rooms
    /// </summary>
    public interface IRoomsData
    {
        /// <summary>
        /// Created rooms
        /// </summary>
        DbSet<Room> Rooms { get; }

        int SaveChanges();
        int SaveChanges(bool acceptAllChangesOnSuccess);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, 
            CancellationToken cancellationToken = default);
    }
}

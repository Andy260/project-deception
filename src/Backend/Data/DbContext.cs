using Microsoft.EntityFrameworkCore;
using ProjectDeception.Backend.Models;
using DbContextBase = Microsoft.EntityFrameworkCore.DbContext;

namespace ProjectDeception.Backend.Data
{
    /// <summary>
    /// Database context
    /// </summary>
    public class DbContext : DbContextBase, IRoomsData
    {
        #region Properties

        /// <summary>
        /// Created <see cref="Room"/>s
        /// </summary>
        public DbSet<Room> Rooms { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a new <see cref="DbContext"/>
        /// </summary>
        /// <param name="options">Context options for creating this <see cref="DbContext"/></param>
        public DbContext(DbContextOptions<DbContext> options) : base(options) { }

        #endregion
    }
}

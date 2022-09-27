using System.ComponentModel.DataAnnotations;
using SongContest.Core.Contracts;

namespace SongContest.Core.Entities
{
    /// <inheritdoc cref="IEntityObject" />
    public class EntityObject : IEntityObject
    {
        /// <inheritdoc />
        [Key] public int Id { get; set; }

        /// <inheritdoc />
        [Timestamp] public byte[] RowVersion { get; set; }
    }
}

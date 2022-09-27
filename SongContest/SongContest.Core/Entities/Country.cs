using System.ComponentModel.DataAnnotations;

namespace SongContest.Core.Entities
{
    public partial class Country : EntityObject
    {
        [Required] public string Name { get; set; }
    }
}

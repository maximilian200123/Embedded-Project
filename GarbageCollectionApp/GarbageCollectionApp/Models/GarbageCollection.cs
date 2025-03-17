using System.ComponentModel.DataAnnotations;

namespace GarbageCollectionApp.Models
{
    public class GarbageCollection
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string IdGarbageBin { get; set; } = string.Empty;

        [Required]
        public DateTime CollectionTime { get; set; }
    }
}

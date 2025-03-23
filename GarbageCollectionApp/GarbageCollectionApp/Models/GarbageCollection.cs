using GarbageCollectionApp.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GarbageCollectionApp.Models {
    public class GarbageCollection
    {
        [Key]
        public int Id { get; set; }

        public string IdGarbageBin { get; set; }

        [ForeignKey("IdGarbageBin")]
        public GarbageBin GarbageBin { get; set; }

        public DateTime CollectionTime { get; set; }
    }
}
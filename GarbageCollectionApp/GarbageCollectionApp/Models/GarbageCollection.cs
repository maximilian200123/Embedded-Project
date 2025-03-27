using GarbageCollectionApp.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GarbageCollectionApp.Models {
    public class GarbageCollection
    {
        [Key]
        public int Id { get; set; }

        public string IdGarbageBin { get; set; }

        [ForeignKey("IdGarbageBin")]
        //public GarbageBin GarbageBin { get; set; }

        public DateTime CollectionTime { get; set; }

        public string Address { get; set; }

        public float Latitude { get; set; }

        public float Longitude { get; set; }
    }
}
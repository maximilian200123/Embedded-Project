using System.ComponentModel.DataAnnotations;

namespace GarbageCollectionApp.Models
{
    public class GarbageBin
    {
        [Key]
        public string IdGarbageBin { get; set; }

        public ICollection<GarbageBinCitizen> GarbageBinCitizens { get; set; }
        public ICollection<GarbageCollection> GarbageCollections { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GarbageCollectionApp.Models
{
    public class GarbageBinCitizen
    {

        [ForeignKey("GarbageBin")]
        public string IdGarbageBin { get; set; }
        public GarbageBin GarbageBin { get; set; }

        [ForeignKey("Citizen")]
        public int IdCitizen { get; set; }
        public Citizen Citizen { get; set; }
        public string Address { get; set; }

    }
}

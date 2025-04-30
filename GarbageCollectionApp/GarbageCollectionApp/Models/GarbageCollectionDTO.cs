namespace GarbageCollectionApp.Models
{
    public class GarbageCollectionDTO
    {
        public string IdGarbageBin { get; set; }
        public DateTime CollectionTime { get; set; }
        public int CitizenId { get; set; }
        public string CitizenFirstName { get; set; }
        public string CitizenLastName { get; set; }
        public string Address { get; set; }
    }
}

using GarbageCollectionApp.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class GarbageCollection
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string IdGarbageBin { get; set; }

    [ForeignKey("IdGarbageBin")]
    public GarbageBin GarbageBin { get; set; }

    [Required]
    public DateTime CollectionTime { get; set; }
}

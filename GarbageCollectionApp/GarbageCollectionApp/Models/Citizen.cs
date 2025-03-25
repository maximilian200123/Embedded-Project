using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace GarbageCollectionApp.Models
{
    public class Citizen
    {
        [Key]
        public int Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
        public string Cnp { get; set; }

    }
}
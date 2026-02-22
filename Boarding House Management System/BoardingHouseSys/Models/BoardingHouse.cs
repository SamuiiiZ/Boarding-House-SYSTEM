using System;

namespace BoardingHouseSys.Models
{
    public class BoardingHouse
    {
        public int Id { get; set; }
        public int OwnerId { get; set; } // Linked to Users table (Role = Admin/Owner)
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Rules { get; set; }
        public string? Amenities { get; set; }
        public string? ImagePath1 { get; set; }
        public string? ImagePath2 { get; set; }
        public string? ImagePath3 { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Helper property (not in DB)
        public string OwnerName { get; set; } = string.Empty;
    }
}

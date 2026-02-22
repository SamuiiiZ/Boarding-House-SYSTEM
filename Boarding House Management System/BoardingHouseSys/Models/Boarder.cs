namespace BoardingHouseSys.Models
{
    public class Boarder
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string? FullName { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public int? RoomId { get; set; }
        public int? BoardingHouseId { get; set; }
        public string? ProfilePicturePath { get; set; }
        public bool IsActive { get; set; }
        
        // Navigation properties (for display)
        public string? RoomNumber { get; set; } 
        public string? BoardingHouseName { get; set; }
    }
}

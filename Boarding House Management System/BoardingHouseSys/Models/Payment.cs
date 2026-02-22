using System;

namespace BoardingHouseSys.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int BoarderId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string MonthPaid { get; set; } = string.Empty;
        public int YearPaid { get; set; }
        public string Status { get; set; } = "Pending";
        public string? Notes { get; set; }

        // Extra properties for display
        public string? BoarderName { get; set; }
        public string? RoomNumber { get; set; }
    }
}
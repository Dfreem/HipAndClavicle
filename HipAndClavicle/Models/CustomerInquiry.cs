using System;
namespace HipAndClavicle.Models
{
    public class CustomerInquiry
    {
        public int Id { get; set; }
        public string? CustomerId { get; set; }
        public AppUser? AppUser { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public DateTime DateSent { get; set; }
        public bool IsResolved { get; set; }
    }
}


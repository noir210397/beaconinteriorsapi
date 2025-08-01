﻿
namespace beaconinteriorsapi.Models
{
    public class Order
    {
        public Guid Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string PhoneNumber { get; set; }
        public required string EmailAddress { get; set; }
        public required List<Address> Addresses { get; set; } = [];
        public OrderStatusType Status { get; set; }
        public PaymentStatusType PaymentStatus { get; set; }
        public List<OrderItems> Items { get; set; } = [];
        public string? UserId { get; set; }
        public User? User { get; set; }
        public required string TrackingID { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool HasExpired { get; set; }
        public Order()
        {
            
        }
    }
}

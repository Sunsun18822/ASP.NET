namespace nguyenthithaoxuan.Models
{
    public class OrderTable
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? CartId { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        public User? User { get; set; }
        public Cart? Cart { get; set; }

        public ICollection<OrderDetail>? OrderDetails { get; set; }
        public ICollection<Payment>? Payments { get; set; }
    }

}

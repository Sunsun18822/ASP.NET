namespace nguyenthithaoxuan.Models
{
    public class Cart
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        public User? User { get; set; }
        public ICollection<CartItem>? CartItems { get; set; }
        public ICollection<OrderTable>? Orders { get; set; }
    }

}

namespace Basket.API.Models
{
    public class ShoppingCart
    {
        public string Id { get; set; } = default!;
        public string UserName { get; set; } = default!;
        public List<ShoppingCartItem> Items { get; set; } = [];
        public decimal TotalPrice => Items.Sum(item => item.Price * item.Quantity);
    }

    public class ShoppingCartItem
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = default!;
        public int Quantity { get; set; } = 1;
        public string Color { get; set; } = default!;
        public decimal Price { get; set; }
    }
}

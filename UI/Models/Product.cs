namespace UI.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public double Price { get; set; }
        public IEnumerable<Product_Prop> ProductProperties { get; set; }
    }
}

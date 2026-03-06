namespace MarketView;

public class ProductViewModel
{
    public string Name { get; set; } = null!;
    public string description { get; set; } = null!;
    public decimal Price { get; set; }
    public string ImageUrl { get; set; } = "/images/default.png";

    public ProductViewModel(){}
}
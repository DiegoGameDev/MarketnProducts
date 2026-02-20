namespace MarketView;

public class MarketViewModel
{
    public string Name { get; set; } = null!;
    public string Location { get; set; } = null!;
    public List<ProductViewModel> Products { get; set; } = new();
}
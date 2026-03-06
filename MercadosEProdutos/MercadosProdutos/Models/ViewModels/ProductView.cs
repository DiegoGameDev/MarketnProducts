using DBModel;

namespace ViewModels;

public class ProductView
{
    public int ID {get; set;}
    

    public string productName {get; set;}

    public string? description {get; set;}

    public decimal productPrice {get; set;}

    public Guid MarketID {get; set;}
    public string? imagePath {get; set;}
    public IFormFile image {get; set;}

    public Product ToDB()
    {
        return new Product()
        {
            productName = this.productName,
            description = this.description,
            productPrice = this.productPrice,
            MarketID = this.MarketID,
        };
    }
}
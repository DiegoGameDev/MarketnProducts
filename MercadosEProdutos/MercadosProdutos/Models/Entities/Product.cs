using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MarketView;
using ViewModels;

namespace DBModel;

public class Product
{
    public int ID {get; set;}
    
    [Required]
    public string productName {get; set;}

    public string? description {get; set;}
    [Required]
    public decimal productPrice {get; set;}
    [Required]
    public Guid MarketID {get; set;}
    public string? imagePath {get; set;}
    [NotMapped]
    public IFormFile? image {get; set;}

    public ProductViewModel ToViewModel()
    {
        return new ProductViewModel()
        {
            Name = this.productName,
            description = this.description,
            Price = this.productPrice,
            ImageUrl = this.imagePath
        };
    }
    public ProductView ToView()
    {
        return new ProductView()
        {
            ID = this.ID,
            productName = this.productName,
            description = this.description,
            productPrice = this.productPrice,
            MarketID = this.MarketID,
            imagePath = this.imagePath
        };
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
}
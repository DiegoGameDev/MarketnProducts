using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DBModel;

public class Product
{
    public int ID {get; set;}
    
    [Required]
    public string productName {get; set;}
    [Required]
    public decimal productPrice {get; set;}
    [Required]
    public Guid MarketIdentification {get; set;}
}
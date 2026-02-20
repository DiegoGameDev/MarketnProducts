using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.SignalR;

namespace DBModel;

public class Market
{
    public Guid ID {get; set;}

    [Required]
    public string marketName {get; set;}
    public string description {get; set;}
    
    [Required]
    public string marketLocal {get; set;}

    [Required]
    public string phoneOrEmailContact {get; set;}

    [Required]
    public string cnpj {get; set;}

    public ICollection<MarketAssociated> MarketAssociatedList {get; set;}
}
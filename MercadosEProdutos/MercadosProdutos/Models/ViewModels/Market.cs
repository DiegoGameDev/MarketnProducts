using System.ComponentModel.DataAnnotations;

namespace ViewModels;

public class Market
{
    [Required]
    public string marketName {get; set;}
    public string description {get; set;}
    
    [Required]
    public string marketLocal {get; set;}

    [Required]
    public string phoneOrEmailContact {get; set;}

    [Required]
    public string cnpj {get; set;}

    public DBModel.Market ToModel()
    {
        return new DBModel.Market()
        {
            marketName = this.marketName,
            description = this.description,
            marketLocal = this.marketLocal,
            phoneOrEmailContact = this.phoneOrEmailContact,
            cnpj = this.cnpj,
            marketReviewStatus = Enums.MarketReviewStatus.Pending
        };
    }
}
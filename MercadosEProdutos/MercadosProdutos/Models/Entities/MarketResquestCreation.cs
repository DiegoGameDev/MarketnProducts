using Enums;

namespace DBModel;

public class MarketRequestCreation
{
    public int ID {get; set;}
    public Guid MarketId {get; set;}
    public Market? Market {get; set;}

    public MarketReviewStatus Status {get; set;}
    public string? RejectionReason {get; set;}
    public DateTime CreatedAt {get; set;}
    public DateTime UpdateDate {get; set;}
}
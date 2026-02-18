using Microsoft.EntityFrameworkCore;

namespace DBModel;

public class MarketAssociated
{
    public int ID {get; set;}
    public Guid MarketID {get; set;}
    public Guid UserID {get; set;}
    public string userLevel {get; set;}
}
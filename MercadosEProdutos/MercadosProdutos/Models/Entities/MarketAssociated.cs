using Microsoft.EntityFrameworkCore;

namespace DBModel;

public class MarketAssociated
{
    public int ID {get; set;}
    public Guid MarketID {get; set;}
    public Market Market {get; set;}
    public string UserID {get; set;}
    public User User {get; set;}
    public string userLevel {get; set;}
}
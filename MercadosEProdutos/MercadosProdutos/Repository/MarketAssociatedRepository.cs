using DBContext;
using DBModel;
using Microsoft.EntityFrameworkCore;
using Results;

namespace Repository;

public class MarketAssociatedRepository : IMarketAssociatedRepository
{
    private readonly MarketDBContext _context;

    public MarketAssociatedRepository(MarketDBContext context)
    {
        _context = context;
    }

    public async Task<ResultOperation> AddUserInMarket(User user, Market market)
    {
        MarketAssociated entity = new()
        {
            MarketID = market.ID,
            UserID = user.Id,
            userLevel = "Staff"
        };

        await _context.MarketAssociatedList.AddAsync(entity);
        await _context.SaveChangesAsync();
        return ResultOperation.Ok("Usuario associado à loja");
    }

    public async Task<ResultOperation<IEnumerable<Market>>> GetMarketListByUserId(string id)
    {
        IEnumerable<Market> marketList = await _context.MarketAssociatedList.Where(x => x.UserID == id).Select(x =>x.Market).AsNoTracking().ToListAsync();

        return ResultOperation<IEnumerable<Market>>.Ok(marketList);
    }

    public async Task<ResultOperation<IEnumerable<User>>> GetUserListByMarket(Market market)
    {      
        IEnumerable<User> userList = await _context.MarketAssociatedList.Where(x => x.MarketID == market.ID).Select(x => x.User).AsNoTracking().ToListAsync();

        return ResultOperation<IEnumerable<User>>.Ok(userList);
    }
}
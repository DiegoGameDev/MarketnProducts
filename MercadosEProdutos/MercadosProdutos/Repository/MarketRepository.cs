using Microsoft.EntityFrameworkCore;
using DBModel;
using Results;
using Repository;
using DBContext;
using System.Linq.Expressions;

namespace Repository
{
    public class MarketRepository : IMarketRepository
    {
        private readonly MarketDBContext _context;

        public MarketRepository(MarketDBContext context)
        {
            _context = context;
        }

        public async Task<ResultOperation<Market>> AddAsync(Market market)
        {
            try
            {
                await _context.MarketList.AddAsync(market);
                await _context.SaveChangesAsync();

                return ResultOperation<Market>.Ok(market, "Market created successfully.");
            }
            catch (Exception ex)
            {
                return ResultOperation<Market>.Fail(ex.Message);
            }
        }

        public async Task<ResultOperation> UpdateAsync(Market market)
        {
            try
            {
                _context.MarketList.Update(market);
                await _context.SaveChangesAsync();

                return ResultOperation.Ok("Market updated successfully.");
            }
            catch (Exception ex)
            {
                return ResultOperation.Fail(ex.Message);
            }
        }

        public async Task<ResultOperation<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var market = await _context.MarketList.FindAsync(id);

                if (market == null)
                    return ResultOperation<bool>.Fail("Market not found.");

                _context.MarketList.Remove(market);
                await _context.SaveChangesAsync();

                return ResultOperation<bool>.Ok(true, "Market deleted successfully.");
            }
            catch (Exception ex)
            {
                return ResultOperation<bool>.Fail(ex.Message);
            }
        }

        public async Task<ResultOperation<Market>> GetByIdAsync(Guid id)
        {
            var market = await _context.MarketList.FirstOrDefaultAsync(x => x.ID == id);

            if (market == null)
                return ResultOperation<Market>.Fail("Market not found.");

            return ResultOperation<Market>.Ok(market);
        }

        public async Task<ResultOperation<IEnumerable<Market>>> GetAllAsync()
        {
            var markets = await _context.MarketList.ToListAsync();

            if (markets == null)
                return ResultOperation<IEnumerable<Market>>.Fail("Não foi possivel acessar a tabela");

            return ResultOperation<IEnumerable<Market>>.Ok(markets);
        }

        public async Task<ResultOperation<IEnumerable<Market>>> SearchAsync(string search)
        {
            var query = _context.MarketList.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x =>
                    x.marketName.Contains(search) ||
                    x.marketLocal.Contains(search) ||
                    x.phoneOrEmailContact.Contains(search) ||
                    x.cnpj.Contains(search));
            }

            var result = await query.ToListAsync();
            return ResultOperation<IEnumerable<Market>>.Ok(result);
        }

        public async Task<ResultOperation<IEnumerable<Market>>> GetPaginatedAsync(int page, int pageSize, string search = null)
        {
            var query = _context.MarketList.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x =>
                    x.marketName.Contains(search) ||
                    x.marketLocal.Contains(search) ||
                    x.phoneOrEmailContact.Contains(search) ||
                    x.cnpj.Contains(search));
            }

            var result = await query
                .OrderBy(x => x.marketName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return ResultOperation<IEnumerable<Market>>.Ok(result);
        }

        public async Task<ResultOperation<Market>> GetByName(string slug)
        {
            var market = await _context.MarketList.FirstOrDefaultAsync(x => x.marketName == slug);

            if (market == null)
                return ResultOperation<Market>.Fail("Nenhuma loja encontrada");

            return ResultOperation<Market>.Ok(market, "Loja encontrada");
        }

        public async Task<ResultOperation<List<Product>>> GetProductsFromMarket(Market market)
        {
            if (market == null)
                return ResultOperation<List<Product>>.Fail("Loja não existe");

            List<Product> products = await _context.ProductList.Where(x => x.MarketID == market.ID).ToListAsync();

            return ResultOperation<List<Product>>.Ok(products);
        }

        public async Task<ResultOperation<IEnumerable<Market>>> GetApprovedMarketListAsync()
        {
            IEnumerable<Market> markets = await _context.MarketList.Where(x => x.marketReviewStatus == Enums.MarketReviewStatus.Approved).AsNoTracking().ToListAsync();

            return ResultOperation<IEnumerable<Market>>.Ok(markets, "Lista de mercados aprovados retornadas com sucesso");
        }
    }
}

using DBModel;
using Results;

namespace Services;

public interface IMyMarketService
{
    Task<ResultOperation<IEnumerable<Market>>> GetMarketsByUserApproved(string userId);
    Task<ResultOperation<Market>> GetMarketById(Guid id);
    Task<ResultOperation> CreateMarketWithRequest(Market market, User userId);
    Task<ResultOperation> UpdateMarket(Market market);

    Task<ResultOperation<ICollection<Product>>> GetProductsInMarket(Guid market);
    Task<ResultOperation<Product>> GetProductById(int id);

    Task<ResultOperation> CreateProduct(Product product);
    Task<ResultOperation> EditProduct(Product product);
    Task<ResultOperation> DeleteProduct(Guid marketId, int productID);
}
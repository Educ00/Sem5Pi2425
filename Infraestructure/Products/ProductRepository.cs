using Sem5Pi2425.Domain.Products;
using Sem5Pi2425.Infrastructure.Shared;

namespace Sem5Pi2425.Infrastructure.Products
{
    public class ProductRepository : BaseRepository<Product, ProductId>,IProductRepository
    {
        public ProductRepository(Sem5Pi2425DbContext context):base(context.Products)
        {
           
        }
    }
}
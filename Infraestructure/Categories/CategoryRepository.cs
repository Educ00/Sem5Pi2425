using Sem5Pi2425.Domain.Categories;
using Sem5Pi2425.Infrastructure.Shared;

namespace Sem5Pi2425.Infrastructure.Categories
{
    public class CategoryRepository : BaseRepository<Category, CategoryId>, ICategoryRepository
    {
    
        public CategoryRepository(Sem5Pi2425DbContext context):base(context.Categories)
        {
           
        }


    }
}
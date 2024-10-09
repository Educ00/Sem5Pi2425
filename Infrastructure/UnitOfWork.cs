using System.Threading.Tasks;
using Sem5Pi2425.Domain.Shared;

namespace Sem5Pi2425.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly Sem5Pi2425DbContext _context;

        public UnitOfWork(Sem5Pi2425DbContext context)
        {
            this._context = context;
        }

        public async Task<int> CommitAsync()
        {
            return await this._context.SaveChangesAsync();
        }
    }
}
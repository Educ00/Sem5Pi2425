using System.Threading.Tasks;

namespace Sem5Pi2425.Domain.Shared
{
    public interface IUnitOfWork
    {
        Task<int> CommitAsync();
    }
}
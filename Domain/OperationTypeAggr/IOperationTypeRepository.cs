using System.Threading.Tasks;
using Sem5Pi2425.Domain.Shared;
using Sem5Pi2425.Domain.SystemUserAggr;

namespace Sem5Pi2425.Domain.OperationTypeAggr
{

    public interface IOperationTypeRepository : IRepository<OperationType, OperationTypeId>
    {
        Task<OperationType> GetByIdAsync(string id);

    }
}
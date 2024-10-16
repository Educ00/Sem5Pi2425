using Microsoft.EntityFrameworkCore;
using Sem5Pi2425.Domain.OperationRequestAggr;
using Sem5Pi2425.Infrastructure.Shared;

namespace Sem5Pi2425.Infrastructure.OperationRequestInfra;

public class OperationRequestRepository : BaseRepository<OperationRequest, OperationRequestId>, IOperationRequestRepository {
    public OperationRequestRepository(Sem5Pi2425DbContext context) : base(context.OperationRequests) { }
}
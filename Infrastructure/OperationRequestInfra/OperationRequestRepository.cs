using Microsoft.EntityFrameworkCore;
using Sem5Pi2425.Domain.OperationRequestAggr;
using Sem5Pi2425.Infrastructure.Shared;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sem5Pi2425.Infrastructure.OperationRequestInfra;

public class OperationRequestRepository : BaseRepository<OperationRequest, OperationRequestId>, IOperationRequestRepository {
    public OperationRequestRepository(Sem5Pi2425DbContext context) : base(context.OperationRequests) { }
    
    
    public new async Task<List<OperationRequest>> GetAllAsync() {
        return await this.Objs
            .Include(p => p.Doctor)
            .Include(p => p.Patient)
            .Include(p => p.OperationType)            
            .Include(p => p.Patient.User)
            .ToListAsync();
    }

    public new async Task<OperationRequest> GetByIdAsync(OperationRequestId id) {
        return await this.Objs
            .Include(p => p.Doctor)
            .Include(p => p.Patient)
            .Include(p => p.OperationType)
            .Include(p => p.Patient.User)
            .FirstOrDefaultAsync(x => id.Equals(x.Id));
    }

    public new async Task<List<OperationRequest>> GetByIdsAsync(List<OperationRequestId> ids) {
        return await this.Objs
            .Include(p => p.Doctor)
            .Include(p => p.Patient)
            .Include(p => p.OperationType)            
            .Include(p => p.Patient.User)
            .Where(x => ids.Contains(x.Id))
            .ToListAsync();
    }
    
}
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sem5Pi2425.Domain.OperationTypeAggr;
using Sem5Pi2425.Infrastructure.Shared;

namespace Sem5Pi2425.Infrastructure.OperationTypeInfra;

public class OperationTypeRepository : BaseRepository<OperationType, OperationTypeId>, IOperationTypeRepository{
    public OperationTypeRepository(Sem5Pi2425DbContext context) : base(context.OperationTypes) { }
}
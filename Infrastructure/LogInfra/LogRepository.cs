using Microsoft.EntityFrameworkCore;
using Sem5Pi2425.Domain.LogAggr;
using Sem5Pi2425.Infrastructure.Shared;

namespace Sem5Pi2425.Infrastructure.LogInfra;

public class LogRepository : BaseRepository<Log, LogId>, ILogRepository {
    public LogRepository(Sem5Pi2425DbContext context) : base(context.Logs) { }
}
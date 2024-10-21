using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Sem5Pi2425.Domain.Shared;
using Sem5Pi2425.Domain.SystemUserAggr;

namespace Sem5Pi2425.Domain.LogAggr;

public class LogService {
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogRepository _logRepository;
    
    public LogService(IUnitOfWork unitOfWork, ILogRepository logRepository) {
        this._unitOfWork = unitOfWork;
        this._logRepository = logRepository;
    }
    
    public async void AddLogAsync(LogDto logDto) {
        Enum.TryParse<Type>(logDto.Type, out var type);
        var log = new Log(type, logDto.Value, new UserId(logDto.UserId));
        await this._logRepository.AddAsync(log);
        await this._unitOfWork.CommitAsync();
    }

    public async Task<IEnumerable<LogDto>> GetAllAsync() {
        var logs = await this._logRepository.GetAllAsync();
        return logs.Select(a => new LogDto(a.Id.Value, a.Type.ToString(), a.Value, a.Id.Value,a.Timestamp.ToString(CultureInfo.InvariantCulture))).ToList();
    }
}
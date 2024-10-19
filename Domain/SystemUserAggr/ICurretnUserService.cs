using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Sem5Pi2425.Domain.SystemUserAggr;

namespace Sem5Pi2425.Domain.SystemUserAggr

{
    public interface ICurrentUserService
    {
        string UserId { get; }
        string Username { get; }
    }
}



namespace Sem5Pi2425.Domain.Shared
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        public string Username => _httpContextAccessor.HttpContext?.User?.Identity?.Name;
    }
}